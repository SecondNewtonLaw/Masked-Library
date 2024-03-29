using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Masked.Scraper;

internal static class Utilities {
    private static readonly Dictionary<string, string> urlText = new() {
        ["%3A"] = ":",
        ["%2F"] = "/",
        ["%3F"] = "?",
        ["%2D"] = "-",
        ["%40"] = "@",
        ["%3D"] = "=",
        ["%2B"] = "+"
    };

    private static readonly Dictionary<string, string> fixUpChars = new() {
        ["&#xE1;"] = "á",
        ["&#xE9;"] = "é",
        ["&#xF3;"] = "ó",
        ["&#x2013;"] = "–",
        ["&#x2014;"] = "—",
        ["&quot;"] = "\"",
        ["&#xFA;"] = "ú",
        ["&#xED;"] = "í",
        ["&#xF1;"] = "ñ",
        ["&#xBF;"] = "¿",
        ["&#xA1;"] = "¡",
        ["&apos;"] = "\'",
        ["&amp;"] = "&",
        ["&#xA0;"] = " ",
        ["&lt;"] = "<",
        ["&gt;"] = ">",
        ["&copy;"] = "©",
        ["&reg;"] = "®",
        ["&euro;"] = "€",
        ["&yen;"] = "¥",
        ["&pound;"] = "£",
        ["&cent;"] = "¢"
    };

    public static void FixURL(ref string dirty, bool reverse) {
        Span<KeyValuePair<string, string>> urlTextSpan = urlText.ToArray();
        ref var searchSpace = ref MemoryMarshal.GetReference(urlTextSpan);
        if (!reverse)
            for (var i = 0; i < urlText.Count; i++) {
                var obj = Unsafe.Add(ref searchSpace, i);
                dirty = dirty.Replace(obj.Key, obj.Value);
            }
        else
            for (var i = 0; i < urlText.Count; i++) {
                var obj = Unsafe.Add(ref searchSpace, i);
                dirty = dirty.Replace(obj.Value, obj.Key);
            }
    }

    public static void FixString(ref string dirty, bool reverse) {
        Span<KeyValuePair<string, string>> charsSpan = urlText.ToArray();
        ref var searchSpace = ref MemoryMarshal.GetReference(charsSpan);
        if (!reverse)
            for (var i = 0; i < fixUpChars.Count; i++) {
                KeyValuePair<string, string> obj = Unsafe.Add(ref searchSpace, i);
                dirty = dirty.Replace(obj.Key, obj.Value);
            }
        else
            for (var i = 0; i < fixUpChars.Count; i++) {
                KeyValuePair<string, string> obj = Unsafe.Add(ref searchSpace, i);
                dirty = dirty.Replace(obj.Value, obj.Key);
            }
    }

    /// <summary>
    /// Static collection of UserAgents used for Scraping.
    /// </summary>
    private static readonly string[] userAgents = new string[] {
        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9",
        "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:15.0) Gecko/20100101 Firefox/15.0.1",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36 Edg/100.0.1185.39",
        "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36"
    };

    public static string GetRandomUserAgent() {
        return userAgents[Random.Shared.NextInt64(0, userAgents.Length)];
    }
}