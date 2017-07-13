using System.Collections.Generic;

namespace TradingPlatform.Models.DirectoryModel
{
    /// <summary>
    /// Справочник
    /// </summary>
    public class DirectoryViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string uk { get; set; }
        public string ru { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionUk { get; set; }
        public int? ParentId { get; set; }
        public List<DirectoryValueViewModel> Values { get; set; }
    }

    /// <summary>
    /// Значения справочника
    /// </summary>
    public class DirectoryValueViewModel
    {
        public int Id { get; set; }
        public int DirectoryId { get; set; }
        public string Code { get; set; }
        public string uk { get; set; }
        public string ru { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionUk { get; set; }
        public int? ParentId { get; set; }
        public List<DependentValueViewModel> DependentValues { get; set; }

    }

    public class DependentValueViewModel
    {
        public int Id { get; set; }
        public int DirectoryId { get; set; }
        public string Code { get; set; }
        public string uk { get; set; }
        public string ru { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionUk { get; set; }
        public int? ParentId { get; set; }
        public List<SecondDependentValueViewModel> AnotherDependencies { get; set; }

    }
    public class SecondDependentValueViewModel
    {
        public int Id { get; set; }
        public int DirectoryId { get; set; }
        public string Code { get; set; }
        public string uk { get; set; }
        public string ru { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionUk { get; set; }

    }


    public class KnockoutDir
    {
        public List<DirectoryViewModel>  Dirs { get; set; }
        public void AddDir()
        {
            Dirs.Add(new DirectoryViewModel());
        }

        public void RemoveItem(int index)
        {
            Dirs.RemoveAt(index);
        }

        public void Save()
        {
          
        }
    }
}