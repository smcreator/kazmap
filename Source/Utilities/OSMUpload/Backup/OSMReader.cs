using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Xml;

namespace OSMUpload
{
    class OSMReader : IDisposable
    {
        readonly XmlReader _reader;
        readonly SQLiteConnection _backend;
        readonly System.IO.Stream _source;
        readonly Dictionary<string, int> _tagType;
        readonly Dictionary<int,bool> _tagFulltext;
        SQLiteCommand _addNode,_addTagType, _addNodeTag;
        SQLiteCommand _addWay, _addNodeInWay, _addWayTag;
        SQLiteCommand _addRel, _addNodeInRel, _addRelTag;
        SQLiteCommand _addWayTextRef;
        SQLiteTransaction _current;
        int _ntxTagType;
        long _nxtDocID;

        public delegate void NotifyProgressDlg(OSMReader reader);

        private OSMReader(System.IO.Stream source,XmlReader reader, SQLiteConnection backend)
        {
            _reader = reader;
            _backend = backend;
            _source = source;
            _tagType = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            _tagFulltext = new Dictionary<int, bool>();

            Top = -180;
            Bottom = 180;
            Left = 180;
            Right = -180;
        }

        public void Dispose()
        {
            if (_current != null) 
            {
                _current.Commit();
                _current = null; 
            }
            foreach (var command in new[] { _addNode, _addNodeTag, _addTagType, 
                _addWay, _addNodeInWay, _addWayTag, _addRel, _addNodeInRel, 
                _addRelTag,_addWayTextRef})
            {
                if(command!=null) command.Dispose();
            }
            _addNode = _addNodeTag = _addTagType = null;
            _addWay = _addNodeInWay = _addWayTag = null;
            _addRel = _addNodeInRel = _addRelTag = null;
            _addWayTextRef = null;
        }

        static void RunNonQuery(SQLiteCommand cmd)
        {
            for(var i=0;i<4;++i)
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("RunNonQuery failed [{0}] : {1}",i,ex);
                }
            }
        }

        void SetupDatabase()
        {
            using(var cmd = _backend.CreateCommand())
            {
                cmd.CommandText= @"
CREATE VIRTUAL TABLE NODES USING rtree(
   id,              -- Integer primary key
   minX, maxX,      -- Minimum and maximum X coordinate
   minY, maxY       -- Minimum and maximum Y coordinate
);             
CREATE TABLE TagType(id integer not null primary key,name text not null);
CREATE TABLE MapInfos(key text  not null primary key,value text not null);

CREATE TABLE Way(wayId integer not null primary key);
CREATE TABLE WayNODE(wayId integer not null,nodeId integer not null,pos integer not null);

CREATE INDEX WayNODE_wayIds  ON WayNODE(wayId);
CREATE INDEX WayNODE_nodeIds ON WayNODE(nodeId);

CREATE TABLE Relation(relId integer not null primary key);
CREATE TABLE RelNODE(relId integer not null,itemId integer not null,pos integer not null,type integer not null);

CREATE INDEX RelNODE_relIds  ON RelNODE(relId);
CREATE INDEX RelNODE_itemIds ON RelNODE(itemId);

CREATE TABLE NodeTAG(nodeId integer not null,type integer not null,value not null);
CREATE TABLE WayTAG(wayId integer not null,type integer not null,value not null);
CREATE TABLE RelTAG(relId integer not null,type integer not null,value not null);

CREATE INDEX NodeTAG_Ids ON NodeTAG(nodeId);
CREATE INDEX WayTAG_Ids ON WayTAG(wayId);
CREATE INDEX RelTAG_Ids ON RelTAG(relId);

CREATE VIRTUAL TABLE WayNames USING fts4(value);
CREATE TABLE WayNamesReference(docid integer not null primary key, wayId integer not null);
";
                RunNonQuery(cmd);
            }

            _addNode = _backend.CreateCommand();
            _addNode.CommandText = "INSERT INTO NODES VALUES(@id,@lng,@lng,@lat,@lat)";
            _addNode.Parameters.Add("@id", DbType.Int64);
            _addNode.Parameters.Add("@lat", DbType.Double);
            _addNode.Parameters.Add("@lng", DbType.Double);
            _addNode.Prepare();

            _addTagType = _backend.CreateCommand();
            _addTagType.CommandText = "INSERT INTO TagType VALUES(@id,@name)";
            _addTagType.Parameters.Add("@id", DbType.Int32);
            _addTagType.Parameters.Add("@name", DbType.String);
            _addTagType.Prepare();

            _addNodeTag = _backend.CreateCommand();
            _addNodeTag.CommandText = "INSERT INTO NodeTAG VALUES(@id,@type,@value)";
            _addNodeTag.Parameters.Add("@id", DbType.Int64);
            _addNodeTag.Parameters.Add("@type", DbType.Int32);
            _addNodeTag.Parameters.Add("@value", DbType.String);
            _addNodeTag.Prepare();

            _addWay = _backend.CreateCommand();
            _addWay.CommandText = "INSERT INTO Way VALUES(@id)";
            _addWay.Parameters.Add("@id", DbType.Int64);
            _addWay.Prepare();
                
            _addNodeInWay = _backend.CreateCommand();
            _addNodeInWay.CommandText = "INSERT INTO WayNODE VALUES(@id,@node,@pos)";
            _addNodeInWay.Parameters.Add("@id", DbType.Int64);
            _addNodeInWay.Parameters.Add("@node", DbType.Int64);
            _addNodeInWay.Parameters.Add("@pos", DbType.Int32);
            _addNodeInWay.Prepare();

            _addWayTag = _backend.CreateCommand();
            _addWayTag.CommandText = "INSERT INTO WayTAG VALUES(@id,@type,@value)";
            _addWayTag.Parameters.Add("@id", DbType.Int64);
            _addWayTag.Parameters.Add("@type", DbType.Int32);
            _addWayTag.Parameters.Add("@value", DbType.String);
            _addWayTag.Prepare();

            _addRel = _backend.CreateCommand();
            _addRel.CommandText = "INSERT INTO Relation VALUES(@id)";
            _addRel.Parameters.Add("@id", DbType.Int64);
            _addRel.Prepare();

            _addNodeInRel = _backend.CreateCommand();
            _addNodeInRel.CommandText = "INSERT INTO RelNODE VALUES(@id,@node,@pos,@type)";
            _addNodeInRel.Parameters.Add("@id",     DbType.Int64);
            _addNodeInRel.Parameters.Add("@node",   DbType.Int64);
            _addNodeInRel.Parameters.Add("@pos",    DbType.Int32);
            _addNodeInRel.Parameters.Add("@type",   DbType.Int32);
            _addNodeInRel.Prepare();


            _addRelTag = _backend.CreateCommand();
            _addRelTag.CommandText = "INSERT INTO RelTAG VALUES(@id,@type,@value)";
            _addRelTag.Parameters.Add("@id",    DbType.Int64);
            _addRelTag.Parameters.Add("@type",  DbType.Int32);
            _addRelTag.Parameters.Add("@value", DbType.String);
            _addRelTag.Prepare();

            _addWayTextRef = _backend.CreateCommand();
            _addWayTextRef.CommandText = "INSERT INTO WayNames (docid,value) VALUES (@docid,@value);INSERT INTO WayNamesReference VALUES(@docid,@wayid)";
            _addWayTextRef.Parameters.Add("@docid", DbType.Int64);
            _addWayTextRef.Parameters.Add("@wayid", DbType.Int64);
            _addWayTextRef.Parameters.Add("@value", DbType.String);
            _addWayTextRef.Prepare();
        }

        public double Top { get; private set; }
        public double Left { get; private set; }
        public double Bottom { get; private set; }
        public double Right { get; private set; }


        long ProcessNodeTag()
        {
            if (!_reader.HasAttributes) return -1; //< Invalid

            var nodeId = 0L;
            double lat = double.NaN, lng = double.NaN;

            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                switch (nme.Length)
                {
                    case 2:
                        if (nme[0] == 'i' && nme[1] == 'd') nodeId = Convert.ToInt64(_reader.Value);
                        break;
                    case 3:
                        if (nme[0] != 'l') break;
                        if (nme[1] == 'a' && nme[2] == 't') lat = Convert.ToDouble(_reader.Value);
                        else if (nme[1] == 'o' && nme[2] == 'n') lng = Convert.ToDouble(_reader.Value);
                        break;
                }
            }

            if (double.IsNaN(lat) || double.IsNaN(lng)) return -1;

            _addNode.Parameters[0].Value = nodeId;
            _addNode.Parameters[1].Value = lat;
            _addNode.Parameters[2].Value = lng;

            Top = Math.Max(lat, Top);
            Bottom = Math.Min(lat, Bottom);
            Left = Math.Min(lng, Left);
            Right = Math.Max(lng, Right);

            RunNonQuery(_addNode);
            return nodeId;
        }

        long ProcessWayTag()
        {
            if (!_reader.HasAttributes) return -1; //< Invalid
            var wayId = 0L;
            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                if(nme.Length==2)
                    if (nme[0] == 'i' && nme[1] == 'd') wayId = Convert.ToInt64(_reader.Value);
            }

            _addWay.Parameters[0].Value = wayId;
            RunNonQuery(_addWay);
            return wayId;
        }

        long ProcessRelTag()
        {
            if (!_reader.HasAttributes) return -1; //< Invalid
            var wayId = 0L;
            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                if (nme.Length == 2)
                    if (nme[0] == 'i' && nme[1] == 'd') wayId = Convert.ToInt64(_reader.Value);
            }

            _addRel.Parameters[0].Value = wayId;
            RunNonQuery(_addRel);
            return wayId;
        }

        void ProcessNdTag(long wayId,int counter)
        {
            if (!_reader.HasAttributes) return; //< Invalid
            var nodeId = 0L;
            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                if (nme.Length == 3)
                    if (nme[0] == 'r' && nme[1] == 'e' && nme[2] == 'f')
                        nodeId = Convert.ToInt64(_reader.Value);
            }

            _addNodeInWay.Parameters[0].Value = wayId;
            _addNodeInWay.Parameters[1].Value = nodeId;
            _addNodeInWay.Parameters[2].Value = counter;
            RunNonQuery(_addNodeInWay);
        }

        void ProcessMemberTag(long relId,int counter)
        {
            if (!_reader.HasAttributes) return; //< Invalid
            var nodeId = 0L;
            var type = LastProcessNode.Invalid;
            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                if (nme.Length == 3)
                {
                    if (nme[0] == 'r' && nme[1] == 'e' && nme[2] == 'f')
                        nodeId = Convert.ToInt64(_reader.Value);
                }
                else if (nme.Length == 4)
                {
                    if (nme[0] == 't' && nme[1] == 'y' && nme[2] == 'p' && nme[3] == 'e')
                        switch (_reader.Value[0])
                        {
                            case 'n':
                                type = LastProcessNode.Node;
                                break;
                            case 'w':
                                type = LastProcessNode.Way;
                                break;
                        }
                    
                }
            }

            _addNodeInRel.Parameters[0].Value = relId;
            _addNodeInRel.Parameters[1].Value = nodeId;
            _addNodeInRel.Parameters[2].Value = counter;
            switch (type)
            {
                case LastProcessNode.Node:
                    _addNodeInRel.Parameters[3].Value = 0;
                    break;
                case LastProcessNode.Way:
                    _addNodeInRel.Parameters[3].Value = 1;
                    break;
                default:
                    return;

            }
            RunNonQuery(_addNodeInRel);
        }


        void ProcessTagOf(LastProcessNode source,long id)
        {
            int tagType;
            string strTagType=null,tagValue=null;

            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                if (nme.Length!=1) continue;
                switch (nme[0])
                {
                    case 'k':
                        strTagType = _reader.Value;
                        break;
                    case 'v':
                        tagValue = _reader.Value;
                        break;
                }
            }

            if(strTagType==null || tagValue==null) return;
            if(!_tagType.TryGetValue(strTagType,out tagType))
            {
                tagType = ++_ntxTagType;
                _addTagType.Parameters[0].Value = tagType;
                _addTagType.Parameters[1].Value = strTagType;
                RunNonQuery(_addTagType);

                _tagType.Add(strTagType,tagType);

                switch (strTagType.ToLower())
                {
                    case "name":
                    case "ref": 
                    case "alt_name":
                    case "postal_code":
                    case "addr:postcode":
                        _tagFulltext.Add(tagType, true); break;
                }
            }


            SQLiteCommand cmd = null;
            switch (source)
            {
                case LastProcessNode.Node:
                    cmd = _addNodeTag;
                    break;
                case LastProcessNode.Way:
                    cmd = _addWayTag;
                    if (_tagFulltext.ContainsKey(tagType))
                    {
                        _addWayTextRef.Parameters[0].Value = ++_nxtDocID;
                        _addWayTextRef.Parameters[1].Value = id;
                        _addWayTextRef.Parameters[2].Value = tagValue;
                        _addWayTextRef.ExecuteNonQuery();
                    }
                    break;
                case LastProcessNode.Relation:
                    cmd = _addRelTag;
                    break;

                default:
                    return;
            }

            cmd.Parameters[0].Value = id;
            cmd.Parameters[1].Value = tagType;
            cmd.Parameters[2].Value = tagValue;
            RunNonQuery(cmd);
        }

        private enum LastProcessNode
        {
            Node,
            Way,
            Relation,
            Invalid
        }

        public long TotalNodes { get; private set; }
        public long TotalWays { get; private set; }
        public long TotalRelations { get; private set; }
        public int PermilCompletion { get; private set; }

        void RollTransaction()
        {
            if(_current!=null) _current.Commit();
            _current = _backend.BeginTransaction();
            foreach (var command in new[] { _addNode, _addNodeTag, _addTagType, 
                _addWay, _addNodeInWay, _addWayTag, _addRel, _addNodeInRel, 
                _addRelTag,_addWayTextRef})
            {
                if (command != null) command.Transaction = _current;
            }            
        }

        void Process(NotifyProgressDlg progressDlg)
        {
            var lastId = 0L;
            var lastType = LastProcessNode.Invalid;
            var counter = 0;

            RollTransaction();
            for (var total = 0; _reader.Read(); ++total)
            {
                if ((total % 10000) == 0 && total>0)
                {
                    PermilCompletion = (int)((_source.Position * 1000) / _source.Length);
                    RollTransaction();
                    if (progressDlg != null) progressDlg(this);
                }

                if (!_reader.IsStartElement()) continue;

                switch (_reader.Name)
                {
                    case "node":
                        ++TotalNodes;
                        lastId = ProcessNodeTag();
                        lastType = lastId<0 ? LastProcessNode.Invalid : LastProcessNode.Node;
                        break;

                    case "way":
                        ++TotalWays;
                        lastId = ProcessWayTag();
                        lastType = lastId < 0 ? LastProcessNode.Invalid : LastProcessNode.Way;
                        counter = 0;
                        break;

                    case "relation":
                        ++TotalRelations;
                        lastId = ProcessRelTag();
                        lastType = lastId < 0 ? LastProcessNode.Invalid : LastProcessNode.Relation;
                        counter = 0;
                        break;

                    case "nd":
                        if (lastType == LastProcessNode.Way)
                            ProcessNdTag(lastId, ++counter);
                        break;

                    case "member":
                        if (lastType == LastProcessNode.Relation)
                            ProcessMemberTag(lastId, ++counter);
                        break;

                    case "tag":
                        if (lastId == 0) break;
                        ProcessTagOf(lastType,lastId);
                        break;

                    default:
                        Console.WriteLine("Ignore " + _reader.Name);
                        lastId = 0;
                        lastType = LastProcessNode.Invalid;
                        break;
                }
            } 
        }

        void StoreMetadata()
        {
            using(var cmd = _backend.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO MapInfos (key,value) VALUES (@key,@value)";
                var kp = cmd.Parameters.Add("@key",     DbType.String);
                var kv = cmd.Parameters.Add("@value",   DbType.String);
                cmd.Prepare();

                kp.Value = "structure_version";
                kv.Value = "1.0";
                cmd.ExecuteNonQuery();

                kp.Value = "build_by";
                kv.Value = "Laurent Dupuis's OSMUpload";
                cmd.ExecuteNonQuery();

                kp.Value = "build_date";
                kv.Value = DateTime.Now.Ticks.ToString();
                cmd.ExecuteNonQuery();

                kp.Value = "total_nodes";
                kv.Value = TotalNodes.ToString();
                cmd.ExecuteNonQuery();

                kp.Value = "total_ways";
                kv.Value = TotalWays.ToString();
                cmd.ExecuteNonQuery();

                kp.Value = "total_rels";
                kv.Value = TotalRelations.ToString();
                cmd.ExecuteNonQuery();

                kp.Value = "box_top";
                kv.Value = Top.ToString();
                cmd.ExecuteNonQuery();

                kp.Value = "box_left";
                kv.Value = Left.ToString();
                cmd.ExecuteNonQuery();

                kp.Value = "box_right";
                kv.Value = Right.ToString();
                cmd.ExecuteNonQuery();

                kp.Value = "box_bottom";
                kv.Value = Right.ToString();
                cmd.ExecuteNonQuery();
            }
        }


        public static void RunUpload(SQLiteConnection backend,string sourceFile, NotifyProgressDlg progressDlg)
        {
            try
            {

                using (var file = System.IO.File.OpenRead(sourceFile))
                using (var bzs = new Bzip2.BZip2InputStream(file))
                using (var reader = XmlReader.Create(bzs))
                using (var processor = new OSMReader(file, reader, backend))
                {
                    processor.SetupDatabase();
                    processor.Process(progressDlg);
                    processor.StoreMetadata();
                }
            }
            catch (System.IO.EndOfStreamException)
            {
                // Silently ignore this one.
            }
        }
    }
}
