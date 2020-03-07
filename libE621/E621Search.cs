using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
namespace libE621
{
    public class E621
    {
        public E621Result.Results DoSearch(List<string> tags, List<string> blacklist, bool ShowApprovedOnly = false)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "HamAndCheeseBot1.0");
            var res = JsonConvert.DeserializeObject<E621Result.Results>(client.DownloadString(BuildQuery(tags, blacklist)));
            //Apply Fixups
            var Cpy = res; //Can't modify an array while also looping over it, so make a copy.
            int i = 0;
            //This is required because at this time I am unable to determine a way to bypass the blacklist from the site.
            foreach (var item in Cpy.posts)
            {
                if (item.file.url == null)
                {
                    bool caught = false;
                    foreach (var tag in item.tags.general)
                    {
                        if (blacklist.Contains(tag))
                        {
                            caught = true; //Post has a blacklisted tag
                            Cpy.posts[i].blacklisted = true;
                        }
                    }
                    if (!caught)
                    {
                        foreach (var tag in item.tags.artist)
                        {
                            if (blacklist.Contains(tag))
                            {
                                caught = true; //Post has a blacklisted tag
                                Cpy.posts[i].blacklisted = true;
                            }
                        }

                        if (!caught)
                        {
                            foreach (var tag in item.tags.character)
                            {
                                if (blacklist.Contains(tag))
                                {
                                    caught = true; //Post has a blacklisted tag
                                    Cpy.posts[i].blacklisted = true;
                                }
                            }
                            if (!caught)
                            {
                                foreach (var tag in item.tags.species)
                                {
                                    if (blacklist.Contains(tag))
                                    {
                                        caught = true; //Post has a blacklisted tag
                                        Cpy.posts[i].blacklisted = true;
                                    }
                                }

                                if (!caught)
                                {
                                    foreach (var tag in item.tags.copyright)
                                    {
                                        if (blacklist.Contains(tag))
                                        {
                                            caught = true; //Post has a blacklisted tag
                                            Cpy.posts[i].blacklisted = true;
                                        }
                                    }
                                    if (!caught)
                                    {
                                        foreach (var tag in item.tags.lore)
                                        {
                                            if (blacklist.Contains(tag))
                                            {
                                                caught = true; //Post has a blacklisted tag
                                                Cpy.posts[i].blacklisted = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!caught)
                    {
                        //rebuild the URL that was removed using the file hash and file extension
                        string url = "https://static1.e621.net/data/";
                        url += item.file.md5.Substring(0, 2) + "/";
                        url += item.file.md5.Substring(2, 2) + "/";
                        url += item.file.md5;
                        url += "." + item.file.ext;
                        res.posts[i].file.url = url; 
                    }
                }
                i++;
            }

            res.posts.RemoveAll(x => x.blacklisted == true);
            //Remove all of the blacklisted results, no need to return them.

            return res;
        }

        private Uri BuildQuery(List<string> tags, List<string> blacklist)
        {
            //https://e621.net/posts.json?tags=public_use+tags2&blacklist=
            string taglist = "";
            foreach (var t in tags)
            {
                taglist += t + "+";
            }

            
            string blist = "";
            foreach (var b in blacklist)
            {
                blist += b + "+";
            }
            
            //Clean up extra characters on the end.
            taglist = taglist.Substring(0, taglist.Length - 1);
            blist = blist.Substring(0, blist.Length - 1);

            string URL = "https://e621.net/posts.json?tags=" + taglist + "&blacklist=" + blist;
            
            Console.WriteLine(URL);
            return new Uri(URL);
        }
    }
}