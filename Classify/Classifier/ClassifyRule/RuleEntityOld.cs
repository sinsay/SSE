using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classifier.ClassifyRule
{
    /// <summary>
    /// 分类规则实体，此实体对应到现有的舆情分类配置，新版实体未定
    /// </summary>
    public class RuleEntityOld
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public MatchModeStyle MatchMode { get; set; }

        public string TitleRuleExpression { get; set; }

        public string TitleRemoveRuleExpression { get; set; }

        public string ContentRuleExpression { get; set; }

        public string ContentRemoveRuleExpression { get; set; }

        public bool IsSystem { get; set; }

        public DateTime AddOn { get; set; }

        public DateTime UpdateOn { get; set; }

        public string SiteIDS { get; set; }

        public bool IsPause { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        /// <summary>
        /// 0.文章 1.微博 2.文章或微博
        /// 4.QQ    5.QQ或文章  6.QQ或微博或文章
        /// 7.垃圾分类
        /// </summary>
        public int ClassifyType { get; set; }

        /// <summary>
        /// 0.巡查分类 1.事件分类
        /// </summary>
        public int Category { get; set; }
    }

    public enum MatchModeStyle
    { 
        JustTitle = 1, 
        JustContent = 2, 
        TitleOrContent = 3,
        TitleAndContent = 4,    
    };   
}
