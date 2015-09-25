using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

using CourseProject.Models;

using Version = Lucene.Net.Util.Version;

        public static class DataRepository
        {
            public static UserTask Get(string id)
            {
                return GetAll().Find(x => x.UserTaskID.Equals(id));
            }

            public static List<UserTask> GetAll()
            {
                ApplicationDbContext DB = new ApplicationDbContext();
                List<UserTask> list = DB.Tasks.ToList();
                return list;
            }
        }


        public static class LuceneSearch
        {
            private static string _luceneDir =
                Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");

            private static FSDirectory _directoryTemp;

            private static FSDirectory _directory
            {
                get
                {
                    if (_directoryTemp == null)
                    {
                        _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                    }
                    if (IndexWriter.IsLocked(_directoryTemp))
                    {
                        IndexWriter.Unlock(_directoryTemp);
                    }
                    var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                    if (File.Exists(lockFilePath))
                    {
                        File.Delete(lockFilePath);
                    }
                    return _directoryTemp;
                }
            }


            private static void _addToLuceneIndex(UserTask task, IndexWriter writer)
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", task.UserTaskID.ToString()));
                writer.DeleteDocuments(searchQuery);

                // add new index entry
                var doc = new Document();

                // add lucene fields mapped to db fields
                doc.Add(new Field("Id", task.UserTaskID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Name", task.TaskName, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Text", task.TaskText, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Difficulty", task.TaskDifficulty, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Category", task.TaskCategory, Field.Store.YES, Field.Index.ANALYZED));
                // add entry to index
                writer.AddDocument(doc);
            }



            public static void AddUpdateLuceneIndex(IEnumerable<UserTask> sampleDatas)
            {
                // init lucene
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // add data to lucene search index (replaces older entry if any)
                    foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }



            public static void AddUpdateLuceneIndex(UserTask user)
            {
                AddUpdateLuceneIndex(new List<UserTask> { user });
            }

            public static void ClearLuceneIndexRecord(int record_id)
            {
                // init lucene
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entry
                    var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                    writer.DeleteDocuments(searchQuery);

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }


            public static bool ClearLuceneIndex()
            {
                try
                {
                    var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                    using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                    {
                        // remove older index entries
                        writer.DeleteAll();

                        // close handles
                        analyzer.Close();
                        writer.Dispose();
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public static void Optimize()
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    analyzer.Close();
                    writer.Optimize();
                    writer.Dispose();
                }
            }

            private static UserTask _mapLuceneDocumentToData(Document doc)
            {
                return new UserTask
                {
                    UserTaskID = int.Parse(doc.Get("Id")),
                };
            }


            private static Query parseQuery(string searchQuery, QueryParser parser)
            {
                Query query;
                try
                {
                    query = parser.Parse(searchQuery.Trim());
                }
                catch (ParseException)
                {
                    query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
                }
                return query;
            }


            private static IEnumerable<UserTask> _mapLuceneToDataList(IEnumerable<Document> hits)
            {
                return hits.Select(_mapLuceneDocumentToData).ToList();
            }
            private static IEnumerable<UserTask> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
            {
                return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
            }



            private static IEnumerable<UserTask> _search(string searchQuery, string searchField = "")
            {
                // validation
                if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<UserTask>();

                // set up lucene searcher
                using (var searcher = new IndexSearcher(_directory, false))
                {
                    var hits_limit = 1000;
                    var analyzer = new StandardAnalyzer(Version.LUCENE_30);

                    // search by single field
                    if (!string.IsNullOrEmpty(searchField))
                    {
                        var parser = new QueryParser(Version.LUCENE_30, searchField, analyzer);
                        var query = parseQuery(searchQuery, parser);
                        var hits = searcher.Search(query, hits_limit).ScoreDocs;
                        var results = _mapLuceneToDataList(hits, searcher);
                        analyzer.Close();
                        searcher.Dispose();
                        return results;
                    }
                    // search by multiple fields (ordered by RELEVANCE)
                    else
                    {
                        var parser = new MultiFieldQueryParser
                        (Version.LUCENE_30, new[] { "Id", "Name", "Text", "Difficulty", "Category" }, analyzer);
                        var query = parseQuery(searchQuery, parser);
                        var hits = searcher.Search
                        (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                        var results = _mapLuceneToDataList(hits, searcher);
                        analyzer.Close();
                        searcher.Dispose();
                        return results;
                    }
                }
            }



            public static IEnumerable<UserTask> SearchDefault(string input, string fieldName = "")
            {
                return string.IsNullOrEmpty(input) ? new List<UserTask>() : _search(input, fieldName);
            }


            public static IEnumerable<UserTask> Search(string input, string fieldName = "")
            {
                if (string.IsNullOrEmpty(input)) return new List<UserTask>();

                var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
                input = string.Join(" ", terms);

                return _search(input, fieldName);
            }

        }
