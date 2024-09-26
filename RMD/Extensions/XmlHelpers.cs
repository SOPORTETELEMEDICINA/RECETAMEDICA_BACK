using System.Xml.Linq;

public static class XmlHelpers
{
    public static T GetOpenSearchValue<T>(this string xmlContent, string elementName)
    {
        var doc = XDocument.Parse(xmlContent);
        var ns = XNamespace.Get("http://a9.com/-/spec/opensearch/1.1/");
        var element = doc.Descendants(ns + elementName).FirstOrDefault();
        if (element != null && typeof(T) == typeof(int))
        {
            return (T)(object)int.Parse(element.Value);
        }
        return default;
    }
}