using System.Collections.Generic;

namespace SubtitlesParser.Classes
{
    public class SubtitlesFormat
    {
        // Properties -----------------------------------------

        public string Name { get; set; }
        public string Extension { get; set; }


        // Private constructor to avoid duplicates ------------

        private SubtitlesFormat(){}


        // Predefined instances -------------------------------     
        public static SubtitlesFormat WebVttFormat = new SubtitlesFormat()
        {
            Name = "WebVTT",
            Extension = @"\.vtt"
        };
      

        public static List<SubtitlesFormat> SupportedSubtitlesFormats = new List<SubtitlesFormat>()
            {
                WebVttFormat,
            };

    }

    
}
