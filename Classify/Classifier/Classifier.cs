using Classifier.ClassifyRule;
using Indexer;
using Indexer.IndexInterface;
using Query;
using Query.Processor;
using Searcher;
using Searcher.Processor;

namespace Classifier
{
    public class Classifier<T>
    {
        /// <summary>
        /// 检索处理器
        /// </summary>
        public ISearchProcessor SearchProcessor { get; set; }

        /// <summary>
        /// 检索表达式处理器
        /// </summary>
        public IQueryProcessor QueryProcessor { get; set; }

        /// <summary>
        /// 索引目录管理器
        /// </summary>
        public IndexDirectoryManager DirManager { get; set; }

        public Classifier(IQueryProcessor queryProcessor, ISearchProcessor searchProcessor)
        {
            this.SearchProcessor = searchProcessor;
            this.QueryProcessor = queryProcessor;
            this.DirManager = new IndexDirectoryManager(Common.Define.SimpleConfig.IndexRootPath);
        }

        /// <summary>
        /// 根据传递的检索上下文进行检索
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        public void Classify(Searcher.SearchContext<T> context)
        {
            this.SearchProcessor.Execute<T>(context);
        }

        /// <summary>
        /// 根据分类规则进行检索
        /// </summary>
        /// <param name="rule"></param>
        public SearchResult<T> Classify(ClassifyRule.RuleEntityOld rule, IIndexDirectory[] pathes, int pageIndex, int maxSize)
        {
            if(rule == null || maxSize <= 0)
            {
               return null;
            }

            return this.Classify(
                new Query.Expression.Node.QueryNode[] { RuleEntityAnalyserOld.Analyser(rule) },
                pathes, pageIndex, maxSize);            
        }

        /// <summary>
        /// 根据检索表达式进行检索
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="pathes"></param>
        /// <param name="pageIndex"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public SearchResult<T> Classify(Query.Expression.Node.QueryNode[] queries, IIndexDirectory[] pathes, int pageIndex, int maxSize)
        {
            // 构建表达式上下文
            QueryContext qContext = new QueryContext()
            {
                Querys = queries
            };

            // 构建检索上下文
            SearchContext<T> sContext = new SearchContext<T>()
            {
                PageIndex = pageIndex,
                PageSize = maxSize,
                QueryContext = qContext,
                Pathes = pathes
            };

            this.Classify(sContext); // 开始检索
            return sContext.Result;
        }
    }
}