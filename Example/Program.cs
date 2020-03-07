using System;
using System.Collections.Generic;
using libE621;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var search = new libE621.E621();
            List<string> tags = new List<string>();
            tags.Add("");
            //Tags to search for
            
            List<string> blacklist = new List<string>();
            blacklist.Add("");
            //Tags to ignore (Will not return a URL, but will be returned in the search)
            
            var result = search.DoSearch(tags, blacklist);
            //Do the actual query
            
            foreach (var item in result.posts)
            {
                Console.WriteLine(item.file.url + " " + item.rating);
            }
            
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}