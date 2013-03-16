using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;
using KazMap.Data;

namespace OSMUpload
{
    class OSMReader
    {
        private KazMapEntities _db = new KazMapEntities();

        readonly XmlReader _reader;
        //readonly SqlConnection _backend;
        readonly System.IO.Stream _source;
        readonly Dictionary<string, int> _tagType;
        readonly Dictionary<int,bool> _tagFulltext;
        //SqlCommand _addNode,_addTagType, _addNodeTag;
        //SqlCommand _addWay, _addNodeInWay, _addWayTag;
        //SqlCommand _addRel, _addNodeInRel, _addRelTag;
        //SqlCommand _addWayTextRef;
        //SqlCommand _storeMetadata;
        //SqlTransaction _current;
        int _ntxTagType;
        long _nxtDocID;

        public delegate void NotifyProgressDlg(OSMReader reader);

        private OSMReader(System.IO.Stream source,XmlReader reader)
        {
            _reader = reader;
            //_backend = backend;
            _source = source;
            _tagType = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            _tagFulltext = new Dictionary<int, bool>();

            Top = -180;
            Bottom = 180;
            Left = 180;
            Right = -180;
        }

        //public void Dispose()
        //{
        //    //if (_current != null) 
        //    //{
        //    //    _current.Commit();
        //    //    _current = null; 
        //    //}
        //    foreach (var command in new[] { _addNode, _addNodeTag, _addTagType, 
        //        _addWay, _addNodeInWay, _addWayTag, _addRel, _addNodeInRel, 
        //        _addRelTag,_addWayTextRef, _storeMetadata})
        //    {
        //        if(command!=null) command.Dispose();
        //    }
        //    _addNode = _addNodeTag = _addTagType = null;
        //    _addWay = _addNodeInWay = _addWayTag = null;
        //    _addRel = _addNodeInRel = _addRelTag = null;
        //    _addWayTextRef = null;
        //}

        //static void RunNonQuery(SqlCommand cmd)
        //{
        //    for(var i=0;i<4;++i)
        //    {
        //        try
        //        {
        //            cmd.ExecuteNonQuery();
        //            return;
        //        }
        //        catch (SqlException ex)
        //        {
        //            Console.WriteLine("RunNonQuery failed [{0}] : {1}",i,ex);
        //        }
        //    }
        //}

        void SetupDatabase()
        {
//            using(var cmd = _backend.CreateCommand())
//            {
//                cmd.CommandText= @"    
//CREATE VIRTUAL TABLE NODES USING rtree(
//   id,              -- Integer primary key
//   minX, maxX,      -- Minimum and maximum X coordinate
//   minY, maxY       -- Minimum and maximum Y coordinate
//);         
//CREATE TABLE TagType(id int not null primary key,name varchar(max) not null);
//CREATE TABLE MapInfos(key varchar(max) not null primary key,value varchar(max) not null);
//
//CREATE TABLE Way(wayId int not null primary key);
//CREATE TABLE WayNODE(wayId int not null,nodeId int not null,pos int not null);
//
//CREATE INDEX WayNODE_wayIds  ON WayNODE(wayId);
//CREATE INDEX WayNODE_nodeIds ON WayNODE(nodeId);
//
//CREATE TABLE Relation(relId integer not null primary key);
//CREATE TABLE RelNODE(relId integer not null,itemId integer not null,pos integer not null,type integer not null);
//
//CREATE INDEX RelNODE_relIds  ON RelNODE(relId);
//CREATE INDEX RelNODE_itemIds ON RelNODE(itemId);
//
//CREATE TABLE NodeTAG(nodeId integer not null,type integer not null,value not null);
//CREATE TABLE WayTAG(wayId integer not null,type integer not null,value not null);
//CREATE TABLE RelTAG(relId integer not null,type integer not null,value not null);
//
//CREATE INDEX NodeTAG_Ids ON NodeTAG(nodeId);
//CREATE INDEX WayTAG_Ids ON WayTAG(wayId);
//CREATE INDEX RelTAG_Ids ON RelTAG(relId);
//
//CREATE VIRTUAL TABLE WayNames USING fts4(value);
//CREATE TABLE WayNamesReference(docid integer not null primary key, wayId integer not null);
//";
//                RunNonQuery(cmd);
//            }

            //_addNode = _backend.CreateCommand();
            //_addNode.CommandText = "INSERT INTO Nodes VALUES(@id,@lng,@lat)";
            //_addNode.Parameters.Add("@id", DbType.Int64);
            //_addNode.Parameters.Add("@lng", DbType.Double);
            //_addNode.Parameters.Add("@lat", DbType.Double);
            
            ////_addNode.Prepare();

            //_addTagType = _backend.CreateCommand();
            //_addTagType.CommandText = "INSERT INTO TagTypes VALUES(@id,@name)";
            //_addTagType.Parameters.Add("@id", DbType.Int32);
            //_addTagType.Parameters.Add("@name", DbType.String);
            ////_addTagType.Prepare();

            //_addNodeTag = _backend.CreateCommand();
            //_addNodeTag.CommandText = "INSERT INTO NodeTags VALUES(@id,@type,@value)";
            //_addNodeTag.Parameters.Add("@id", DbType.Int64);
            //_addNodeTag.Parameters.Add("@type", DbType.Int32);
            //_addNodeTag.Parameters.Add("@value", DbType.String);
            ////_addNodeTag.Prepare();

            //_addWay = _backend.CreateCommand();
            //_addWay.CommandText = "INSERT INTO Ways VALUES(@id)";
            //_addWay.Parameters.Add("@id", DbType.Int64);
            ////_addWay.Prepare();
                
            //_addNodeInWay = _backend.CreateCommand();
            //_addNodeInWay.CommandText = "INSERT INTO WayNodes VALUES(@id,@node,@pos)";
            //_addNodeInWay.Parameters.Add("@id", DbType.Int64);
            //_addNodeInWay.Parameters.Add("@node", DbType.Int64);
            //_addNodeInWay.Parameters.Add("@pos", DbType.Int32);
            ////_addNodeInWay.Prepare();

            //_addWayTag = _backend.CreateCommand();
            //_addWayTag.CommandText = "INSERT INTO WayTags VALUES(@id,@type,@value)";
            //_addWayTag.Parameters.Add("@id", DbType.Int64);
            //_addWayTag.Parameters.Add("@type", DbType.Int32);
            //_addWayTag.Parameters.Add("@value", DbType.String);
            ////_addWayTag.Prepare();

            //_addRel = _backend.CreateCommand();
            //_addRel.CommandText = "INSERT INTO Relations VALUES(@id)";
            //_addRel.Parameters.Add("@id", DbType.Int64);
            ////_addRel.Prepare();

            //_addNodeInRel = _backend.CreateCommand();
            //_addNodeInRel.CommandText = "INSERT INTO RelationNodes VALUES(@id,@node,@pos,@type)";
            //_addNodeInRel.Parameters.Add("@id",     DbType.Int64);
            //_addNodeInRel.Parameters.Add("@node",   DbType.Int64);
            //_addNodeInRel.Parameters.Add("@pos",    DbType.Int32);
            //_addNodeInRel.Parameters.Add("@type",   DbType.Int32);
            ////_addNodeInRel.Prepare();


            //_addRelTag = _backend.CreateCommand();
            //_addRelTag.CommandText = "INSERT INTO RelationTags VALUES(@id,@type,@value)";
            //_addRelTag.Parameters.Add("@id",    DbType.Int64);
            //_addRelTag.Parameters.Add("@type",  DbType.Int32);
            //_addRelTag.Parameters.Add("@value", DbType.String);
            ////_addRelTag.Prepare();

            //_addWayTextRef = _backend.CreateCommand();
            //_addWayTextRef.CommandText = "INSERT INTO WayNames (NameId,value) VALUES (@docid,@value);INSERT INTO WayNamesReferences VALUES(@docid,@wayid)";
            //_addWayTextRef.Parameters.Add("@docid", DbType.Int64);
            //_addWayTextRef.Parameters.Add("@wayid", DbType.Int64);
            //_addWayTextRef.Parameters.Add("@value", DbType.String);
            ////_addWayTextRef.Prepare();

            //_storeMetadata = _backend.CreateCommand();
            //_storeMetadata.CommandText = "INSERT INTO MapInfos ([akey],[avalue]) VALUES (@key,@value)";
            //var kp = _storeMetadata.Parameters.Add("@key", DbType.String);
            //var kv = _storeMetadata.Parameters.Add("@value", DbType.String);

        }

        public double Top { get; private set; }
        public double Left { get; private set; }
        public double Bottom { get; private set; }
        public double Right { get; private set; }


        long ReadNode()
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

            var node = new Nodes
            {
                Id = nodeId,
                Longitude = lng,
                Latitude = lat,                
            };

            //_addNode.Parameters[0].Value = nodeId;
            //_addNode.Parameters[1].Value = lng;
            //_addNode.Parameters[2].Value = lat;

            Top = Math.Max(lat, Top);
            Bottom = Math.Min(lat, Bottom);
            Left = Math.Min(lng, Left);
            Right = Math.Max(lng, Right);

            //RunNonQuery(_addNode);
            _db.Nodes.AddObject(node);

            return nodeId;
        }

        long ReadWay()
        {
            if (!_reader.HasAttributes) return -1; //< Invalid
            var wayId = 0L;
            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                if(nme.Length == 2)
                    if (nme[0] == 'i' && nme[1] == 'd') wayId = Convert.ToInt64(_reader.Value);
            }

            //_addWay.Parameters[0].Value = wayId;
            var way = new Ways { WayId = wayId };
            //RunNonQuery(_addWay);
            _db.Ways.AddObject(way);
            return wayId;
        }

        long ReadRelation()
        {
            if (!_reader.HasAttributes) return -1; //< Invalid
            var relationId = 0L;
            while (_reader.MoveToNextAttribute())
            {
                var nme = _reader.Name;
                if (nme.Length == 2)
                    if (nme[0] == 'i' && nme[1] == 'd') relationId = Convert.ToInt64(_reader.Value);
            }

            //_addRel.Parameters[0].Value = wayId;
            var relation = new Relations { RelationId = relationId };
            //RunNonQuery(_addRel);
            _db.Relations.AddObject(relation);
            return relationId;
        }

        void ReadWayNode(long wayId,int counter)
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

            //_addNodeInWay.Parameters[0].Value = wayId;
            //_addNodeInWay.Parameters[1].Value = nodeId;
            //_addNodeInWay.Parameters[2].Value = counter;
            var wayNode = new WayNodes
            {
                WayId = wayId,
                NodeId = nodeId,
                Position = counter
            };
            //RunNonQuery(_addNodeInWay);
            _db.WayNodes.AddObject(wayNode);
        }

        void ReadRelationNode(long relId,int counter)
        {
            if (!_reader.HasAttributes) return; //< Invalid
            var nodeId = 0L;
            var type = LastReadedNode.Invalid;
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
                                type = LastReadedNode.Node;
                                break;
                            case 'w':
                                type = LastReadedNode.Way;
                                break;
                        }
                    
                }
            }

            //_addNodeInRel.Parameters[0].Value = relId;
            //_addNodeInRel.Parameters[1].Value = nodeId;
            //_addNodeInRel.Parameters[2].Value = counter;
            var relationNode = new RelationNodes
            {
                RelationId = relId,
                ItemId = nodeId,
                Position = counter
            };
            switch (type)
            {
                case LastReadedNode.Node:
                    //_addNodeInRel.Parameters[3].Value = 0;
                    //relationNode.TypeId = 0;
                    break;
                case LastReadedNode.Way:
                    //_addNodeInRel.Parameters[3].Value = 1;
                    //relationNode.TypeId = 1;
                    break;
                default:
                    return;

            }
            //RunNonQuery(_addNodeInRel);
            _db.RelationNodes.AddObject(relationNode);
        }

        void ReadType(LastReadedNode source,long id)
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
                var type = new TagTypes
                {
                    Id = tagType,
                    Name = strTagType
                };
                //_addTagType.Parameters[0].Value = tagType;
                //_addTagType.Parameters[1].Value = strTagType;
                //RunNonQuery(_addTagType);
                _db.TagTypes.AddObject(type);

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


            //SqlCommand cmd = null;
            switch (source)
            {
                case LastReadedNode.Node:
                    //cmd = _addNodeTag;
                    var nodeTag = new NodeTags
                    {
                        NodeId = id,
                        TypeId = tagType,
                        Value = tagValue
                    };
                    _db.NodeTags.AddObject(nodeTag);
                    break;
                case LastReadedNode.Way:
                    //cmd = _addWayTag;
                    var wayTag = new WayTags
                    {
                        WayId = id,
                        TypeId = tagType,
                        Value = tagValue
                    };
                    _db.WayTags.AddObject(wayTag);
                    if (_tagFulltext.ContainsKey(tagType))
                    {                        
                        var wayNameReference = new WayNamesReferences
                        {
                            NameId = ++_nxtDocID,
                            WayId = id
                        };
                        _db.WayNamesReferences.AddObject(wayNameReference);
                        var wayName = new WayNames
                        {
                            NameId = _nxtDocID,
                            Value = tagValue
                        };
                        _db.WayNames.AddObject(wayName);
                        //_addWayTextRef.Parameters[0].Value = ++_nxtDocID;
                        //_addWayTextRef.Parameters[1].Value = id;
                        //_addWayTextRef.Parameters[2].Value = tagValue;
                        //_addWayTextRef.ExecuteNonQuery();
                    }
                    break;
                case LastReadedNode.Relation:
                    //cmd = _addRelTag;
                    var relationTag = new RelationTags
                    {
                        RelationId = id,
                        TypeId = tagType,
                        Value = tagValue
                    };
                    _db.RelationTags.AddObject(relationTag);
                    break;
                default:
                    return;
            }

            //cmd.Parameters[0].Value = id;
            //cmd.Parameters[1].Value = tagType;
            //cmd.Parameters[2].Value = tagValue;
            //RunNonQuery(cmd);
        }

        private enum LastReadedNode
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

        void Read(NotifyProgressDlg progressDlg)
        {
            var lastId = 0L;
            var lastType = LastReadedNode.Invalid;
            var counter = 0;

            //RollTransaction();
            for (var total = 0; _reader.Read(); ++total)
            {
                if ((total % 10000) == 0 && total>0)
                {
                    PermilCompletion = (int)((_source.Position * 1000) / _source.Length);
                    //RollTransaction();
                    if (progressDlg != null) progressDlg(this);
                }

                if (!_reader.IsStartElement()) continue;

                switch (_reader.Name)
                {
                    case "node":
                        ++TotalNodes;
                        lastId = ReadNode();
                        lastType = lastId<0 ? LastReadedNode.Invalid : LastReadedNode.Node;
                        break;

                    case "way":
                        ++TotalWays;
                        lastId = ReadWay();
                        lastType = lastId < 0 ? LastReadedNode.Invalid : LastReadedNode.Way;
                        counter = 0;
                        break;

                    case "relation":
                        ++TotalRelations;
                        lastId = ReadRelation();
                        lastType = lastId < 0 ? LastReadedNode.Invalid : LastReadedNode.Relation;
                        counter = 0;
                        break;

                    case "nd":
                        if (lastType == LastReadedNode.Way)
                            ReadWayNode(lastId, ++counter);
                        break;

                    case "member":
                        if (lastType == LastReadedNode.Relation)
                            ReadRelationNode(lastId, ++counter);
                        break;

                    case "tag":
                        if (lastId == 0) break;
                        ReadType(lastType,lastId);
                        break;

                    default:
                        Console.WriteLine("Ignore " + _reader.Name);
                        lastId = 0;
                        lastType = LastReadedNode.Invalid;
                        break;
                }
            }
            _db.SaveChanges();
        }

        //void StoreMetadata()
        //{
        //    //RollTransaction();
        //    //cmd.Prepare();

        //    _storeMetadata.Parameters[0].Value = "structure_version";
        //    _storeMetadata.Parameters[1].Value = "1.0";
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "build_by";
        //    _storeMetadata.Parameters[1].Value = "Laurent Dupuis's OSMUpload";
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "build_date";
        //    _storeMetadata.Parameters[1].Value = DateTime.Now.Ticks.ToString();
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "total_nodes";
        //    _storeMetadata.Parameters[1].Value = TotalNodes.ToString();
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "total_ways";
        //    _storeMetadata.Parameters[1].Value = TotalWays.ToString();
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "total_rels";
        //    _storeMetadata.Parameters[1].Value = TotalRelations.ToString();
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "box_top";
        //    _storeMetadata.Parameters[1].Value = Top.ToString();
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "box_left";
        //    _storeMetadata.Parameters[1].Value = Left.ToString();
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "box_right";
        //    _storeMetadata.Parameters[1].Value = Right.ToString();
        //    _storeMetadata.ExecuteNonQuery();

        //    _storeMetadata.Parameters[0].Value = "box_bottom";
        //    _storeMetadata.Parameters[1].Value = Right.ToString();
        //    _storeMetadata.ExecuteNonQuery();
        //}

        public static void Read(string sourceFile, NotifyProgressDlg progressDlg)
        {
            try
            {

                using (var file = System.IO.File.OpenRead(sourceFile))
                //using (var bzs = new Bzip2.BZip2InputStream(file))
                using (var reader = XmlReader.Create(file))
                {
                    var osmReader = new OSMReader(file, reader);
                    osmReader.Read(progressDlg);
                }
            }
            catch (System.IO.EndOfStreamException)
            {
                // Silently ignore this one.
            }
        }
    }
}
