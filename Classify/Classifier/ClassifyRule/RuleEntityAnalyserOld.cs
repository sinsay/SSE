using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query.Expression.Node;
using Query.Expression.FieldValue;
using UtilityLib.Converter;

namespace Classifier.ClassifyRule
{
    public static class RuleEntityAnalyserOld
    {
        /// <summary>
        /// 将分类规则解析为检索表达式树
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static QueryNode Analyser(RuleEntityOld rule)
        {
            QueryNode query = null;
            List<QueryNode> notQueries = new List<QueryNode>();

            // 构建非规则
            if (!string.IsNullOrEmpty(rule.TitleRemoveRuleExpression))
            {
                notQueries.Add(new QueryNode("Title", new ImmideatelyValue(rule.TitleRemoveRuleExpression)));
            }
            if (!string.IsNullOrEmpty(rule.ContentRemoveRuleExpression))
            {
                notQueries.Add(new QueryNode("Content", new ImmideatelyValue(rule.ContentRemoveRuleExpression)));
            }
            QueryNode notQuery = new QueryNode(QueryLogic.MUST_NOT, notQueries.ToArray());           
            

            // 构建匹配规则
            QueryLogic logic = rule.MatchMode == MatchModeStyle.TitleAndContent ? QueryLogic.MUST : QueryLogic.SHOULD;
            if (!string.IsNullOrEmpty(rule.TitleRuleExpression) && !string.IsNullOrEmpty(rule.ContentRuleExpression))
            {
                query = new QueryNode(
                                logic,
                                new QueryNode[]
                                {
                                    new QueryNode("Title", new Query.Expression.FieldValue.ImmideatelyValue(rule.TitleRuleExpression)),
                                    new QueryNode("Content", new Query.Expression.FieldValue.ImmideatelyValue(rule.ContentRuleExpression))
                                });
            }
            else
            {
                List<QueryNode> querys = new List<QueryNode>();
                if (!string.IsNullOrEmpty(rule.TitleRuleExpression))
                {
                    querys.Add(new QueryNode("Title", new ImmideatelyValue(rule.TitleRuleExpression)));
                }
                if (!string.IsNullOrEmpty(rule.ContentRuleExpression))
                {
                    querys.Add(new QueryNode("Content", new ImmideatelyValue(rule.ContentRuleExpression)));
                }

                query = new QueryNode(logic, querys.ToArray());
            }

            // 处理其他查询条件，如时间，如站点
            List<QueryNode> otherQuery = new List<QueryNode>();
            if (!string.IsNullOrEmpty(rule.SiteIDS))
            {
                string[] siteIds = null;
                try
                {
                    siteIds = rule.SiteIDS.Split(new char[] { ',' }).ToArray();
                }
                catch (Exception e)
                {
                    Console.WriteLine("站点ID格式有错");
                }

                if (siteIds != null)
                {
                    otherQuery.Add(
                        new QueryNode(
                            "SiteId",
                            new Collection(siteIds)
                        ));
                }
            }

            // 重新设置分类规则的时间
            ReSetRule(rule);

            otherQuery.Add(new QueryNode(
                "PublishOn",
                new Query.Expression.FieldValue.Range(
                    DateTimeConverter.TimeSpan4Minute(rule.BeginDate),
                    DateTimeConverter.TimeSpan4Minute(rule.EndDate),
                    true)));

            return new QueryNode(
                new List<QueryNode>(otherQuery)
                {
                    query,
                    notQuery
                }.ToArray());
        }

        public static void ReSetRule(RuleEntityOld rule)
        {

            if (rule.BeginDate <= RuleConvention.DefaultTime)
            {
                rule.BeginDate = RuleConvention.DefaultTime;
            }

            if (rule.EndDate <= RuleConvention.DefaultTime)
            {
                rule.EndDate = DateTime.Now;
            }
        }
    }
}