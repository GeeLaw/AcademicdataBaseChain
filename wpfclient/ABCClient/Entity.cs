using System.Collections.Generic;

namespace ABCClient
{
    public class Entity
    {
        public string DisplayName { get; set; }
        public string AccountAddress { get; set; }

        public static IReadOnlyList<Entity> Entities { get { return entities.AsReadOnly(); } }

        static List<Entity> entities = new List<Entity>
        {
            new Entity { DisplayName = "Bureau of Education", AccountAddress = "<bureau of education address>" },
            new Entity { DisplayName = "Tsinghua Univeristy", AccountAddress = "<school 1 address>" },
            new Entity { DisplayName = "University of Waterloo", AccountAddress = "<school 2 address>" },
            new Entity { DisplayName = "Gee Law", AccountAddress = "<person 1 address>" },
            new Entity { DisplayName = "Ting Fung Lau", AccountAddress = "<person 2 address>" },
            new Entity { DisplayName = "Wangbin Sun", AccountAddress = "<person 3 address>" },
            new Entity { DisplayName = "Congrong Ma", AccountAddress = "<person 4 address>" },
            new Entity { DisplayName = "Jingyi Wang", AccountAddress = "<person 5 address>" },
            new Entity { DisplayName = "Chelsea Liu", AccountAddress = "<person 6 address>" }
        };
    }
}
