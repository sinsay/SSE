SSE
===

small search engine implement

----2013.01.17----
    补充第一份说明，这个项目是三四个月前写的基于Lucene.Net的一个小型通用搜索引擎，
稍迟会补上Demo, 当时的目标是一个通用的索引跟检索架构。
索引方面：
	每增加一种需要索引的文档类型，
都只需要新建一个类，在类的各个字段上标注上索引信息（如是否需要存储，是否需要分词等）
然后又数据源提供实体信息给索引器即可自动索引。
检索方面：
	检索条件由一棵检索的表达式树组合而成，各种检索条件均可由表达式树组成，也可根据
业务需要封装成特定的接口。
索引存储方面：
	使用了按时间跟文档数分割的原则，自动按月份生成索引目录，并且限制每个目录下的最大文档数。
具体这样做的原因，稍迟会以文档形式补上。

