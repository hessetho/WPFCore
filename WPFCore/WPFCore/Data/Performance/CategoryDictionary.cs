using System.Collections.Generic;

namespace WPFCore.Data.Performance
{
    public class CategoryDictionary : SerializableDictionary<string, List<PerformanceItem>>
    {
        public CategoryDictionary()
        {
            this.XmlItemTagName = "Category";
            this.XmlKeyTagName = "Name";
            this.XmlValueTagName = "PerformanceItem";
        }
    }
}