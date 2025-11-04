using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TagAnalyzer.Models;

public class TagInfo
{
    public string Tag { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<string> SourceIds { get; set; } = new();
}

public class TextParser
{
    private static readonly Regex IdPattern = new(@"M\d+_\d{6}");
    private static readonly Regex TagPattern = new(@"【([^】]+)】");

    public static List<TagInfo> ParseText(string text)
    {
        var tagDictionary = new Dictionary<string, TagInfo>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // 跳过拼音行（以制表符开头）
            if (trimmedLine.StartsWith('\t'))
                continue;

            // 查找ID
            var idMatch = IdPattern.Match(trimmedLine);
            if (!idMatch.Success)
                continue;

            var id = idMatch.Value;

            // 查找所有标签
            var tagMatches = TagPattern.Matches(trimmedLine);
            foreach (Match tagMatch in tagMatches)
            {
                var tag = tagMatch.Groups[1].Value;
                
                if (!tagDictionary.ContainsKey(tag))
                {
                    tagDictionary[tag] = new TagInfo
                    {
                        Tag = tag,
                        Count = 0,
                        SourceIds = new List<string>()
                    };
                }

                tagDictionary[tag].Count++;
                if (!tagDictionary[tag].SourceIds.Contains(id))
                {
                    tagDictionary[tag].SourceIds.Add(id);
                }
            }
        }

        return tagDictionary.Values.OrderBy(t => t.Tag).ToList();
    }
}
