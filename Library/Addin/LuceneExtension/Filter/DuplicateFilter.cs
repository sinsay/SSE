    /**
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace LuceneExtension.Filter
{
    public class DuplicateFilter : Lucene.Net.Search.Filter
    {

        String fieldName;

        private static Dictionary<string, KeyValuePair<DateTime,DocIdSet>> DocCache;

        /**
         * KeepMode determines which document id to consider as the master, all others being 
         * identified as duplicates. Selecting the "first occurrence" can potentially save on IO.
         */
        int keepMode = KM_USE_FIRST_OCCURRENCE;
        public static int KM_USE_FIRST_OCCURRENCE = 1;
        public static int KM_USE_LAST_OCCURRENCE = 2;

        /**
         * "Full" processing mode starts by setting all bits to false and only setting bits
         * for documents that contain the given field and are identified as none-duplicates. 

         * "Fast" processing sets all bits to true then unsets all duplicate docs found for the
         * given field. This approach avoids the need to read TermDocs for terms that are seen 
         * to have a document frequency of exactly "1" (i.e. no duplicates). While a potentially 
         * faster approach , the downside is that bitsets produced will include bits set for 
         * documents that do not actually contain the field given.
         * 
         */
        int processingMode = PM_FULL_VALIDATION;
        public static int PM_FULL_VALIDATION = 1;
        public static int PM_FAST_INVALIDATION = 2;

        static DuplicateFilter() 
        {
            DocCache = new Dictionary<string, KeyValuePair<DateTime, DocIdSet>>();
        }

        public DuplicateFilter(String fieldName) :  this(fieldName, KM_USE_LAST_OCCURRENCE, PM_FULL_VALIDATION)
        {
        }


        public DuplicateFilter(String fieldName, int keepMode, int processingMode)
        {
            this.fieldName = fieldName;
            this.keepMode = keepMode;
            this.processingMode = processingMode;
        }

        public override DocIdSet GetDocIdSet(IndexReader reader)
        {
            DocIdSet docs = null;

            // TODO: override by SinSay
            var cache = IdSetCache.GetCache(reader.Directory().ToString(), this.fieldName);
            if (cache != null)
            {
                return cache.Cache;
            }

            if (processingMode == PM_FAST_INVALIDATION)
            {
                docs = FastBits(reader);
            }
            else
            {
                docs = CorrectBits(reader);
            }

            return docs;
        }

        public DocIdSet GetDocIdSetCache(IndexReader reader)
        {
            DocIdSet docs = null;

            if (processingMode == PM_FAST_INVALIDATION)
            {
                docs = FastBits(reader);
            }
            else
            {
                docs = CorrectBits(reader);
            }

            return docs;
        }

        private OpenBitSet CorrectBits(IndexReader reader)
        {
            OpenBitSet bits = new OpenBitSet(reader.MaxDoc()); //assume all are INvalid
            Term startTerm = new Term(fieldName);
            TermEnum te = reader.Terms(startTerm);
            if (te != null)
            {
                Term currTerm = te.Term();
                while ((currTerm != null) && (currTerm.Field() == startTerm.Field())) //term fieldnames are interned
                {
                    int lastDoc = -1;
                    //set non duplicates
                    TermDocs td = reader.TermDocs(currTerm);
                    if (td.Next())
                    {
                        if (keepMode == KM_USE_FIRST_OCCURRENCE)
                        {
                            bits.Set(td.Doc());
                        }
                        else
                        {
                            do
                            {
                                lastDoc = td.Doc();
                            } while (td.Next());
                            bits.Set(lastDoc);
                        }
                    }
                    if (!te.Next())
                    {
                        break;
                    }
                    currTerm = te.Term();
                }
            }
            return bits;
        }

        private OpenBitSet FastBits(IndexReader reader)
        {
            OpenBitSet bits = new OpenBitSet(reader.MaxDoc());
            bits.Set(0, reader.MaxDoc()); //assume all are valid
            Term startTerm = new Term(fieldName);
            TermEnum te = reader.Terms(startTerm);
            if (te != null)
            {
                Term currTerm = te.Term();

                while ((currTerm != null) && (currTerm.Field() == startTerm.Field())) //term fieldnames are interned
                {
                    if (te.DocFreq() > 1)
                    {
                        int lastDoc = -1;
                        //unset potential duplicates
                        TermDocs td = reader.TermDocs(currTerm);
                        td.Next();
                        if (keepMode == KM_USE_FIRST_OCCURRENCE)
                        {
                            td.Next();
                        }
                        do
                        {
                            lastDoc = td.Doc();
                            bits.Clear(lastDoc);
                        } while (td.Next());
                        if (keepMode == KM_USE_LAST_OCCURRENCE)
                        {
                            //restore the last bit
                            bits.Set(lastDoc);
                        }
                    }
                    if (!te.Next())
                    {
                        break;
                    }
                    currTerm = te.Term();
                }
            }
            return bits;
        }

        //    /**
        //     * @param args
        //     * @throws IOException 
        //     * @throws Exception 
        //     */
        //    public static void main(String[] args) 
        //    {
        //        IndexReader r=IndexReader.open("/indexes/personCentricAnon");
        ////		IndexReader r=IndexReader.open("/indexes/enron");
        //        long start=System.currentTimeMillis();
        ////		DuplicateFilter df = new DuplicateFilter("threadId",KM_USE_FIRST_OCCURRENCE, PM_FAST_INVALIDATION);
        ////		DuplicateFilter df = new DuplicateFilter("threadId",KM_USE_LAST_OCCURRENCE, PM_FAST_INVALIDATION);
        //        DuplicateFilter df = new DuplicateFilter("vehicle.vrm",KM_USE_LAST_OCCURRENCE, PM_FAST_INVALIDATION);
        ////		DuplicateFilter df = new DuplicateFilter("title",USE_LAST_OCCURRENCE);
        ////		df.setProcessingMode(PM_SLOW_VALIDATION);
        //        BitSet b = df.bits(r);
        //        long end=System.currentTimeMillis()-start;
        //        System.out.println(b.cardinality()+" in "+end+" ms ");

        //    }


        public String GetFieldName()
        {
            return fieldName;
        }


        public void SetFieldName(String fieldName)
        {
            this.fieldName = fieldName;
        }


        public int GetKeepMode()
        {
            return keepMode;
        }


        public void SetKeepMode(int keepMode)
        {
            this.keepMode = keepMode;
        }


        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if ((obj == null) || (obj.GetType()!= this.GetType()))
                return false;
            DuplicateFilter other = (DuplicateFilter)obj;
            return keepMode == other.keepMode &&
            processingMode == other.processingMode &&
                (fieldName == other.fieldName || (fieldName != null && fieldName.Equals(other.fieldName)));
        }



        public override int GetHashCode()
        {
            int hash = 217;
            hash = 31 * hash + keepMode;
            hash = 31 * hash + processingMode;
            hash = 31 * hash + fieldName.GetHashCode();
            return hash;
        }


        public int GetProcessingMode()
        {
            return processingMode;
        }


        public void SetProcessingMode(int processingMode)
        {
            this.processingMode = processingMode;
        }
    }

    public static class IdSetCache
    {
        public static Dictionary<string, DocCacheEntity> Cache;

        static IdSetCache()
        {
            Cache = new Dictionary<string, DocCacheEntity>();
        }

        public static DocCacheEntity GetCache(string path, string fieldName)
        {
            if (!System.IO.Directory.Exists(path) || !IndexReader.IndexExists(path))
            {
                return null;
            }

            var key = string.Join("@", path, fieldName);
            lock (Cache)
            {
                if (!Cache.ContainsKey(key) || Cache[key].LastChange != IndexReader.LastModified(path))
                {
                    var duplicateFilter = new DuplicateFilter(fieldName);
                    var reader = IndexReader.Open(path);
                    var docSet = duplicateFilter.GetDocIdSetCache(reader);

                    Cache[key] = new DocCacheEntity
                    {
                        Cache = docSet,
                        DocPath = path,
                        FieldName = fieldName,
                        LastChange = IndexReader.LastModified(path)
                    };
                }
            }

            return Cache[key];
        }
    }

    public class DocCacheEntity
    {
        public DocIdSet Cache { get; set; }
        public long LastChange { get; set; }
        public string DocPath { get; set; }
        public string FieldName { get; set; }
    }
}