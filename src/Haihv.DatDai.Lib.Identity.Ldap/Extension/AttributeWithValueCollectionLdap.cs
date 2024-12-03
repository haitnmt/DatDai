using Haihv.DatDai.Lib.Identity.Ldap.Enum;

namespace Haihv.DatDai.Lib.Identity.Ldap.Extension;

public class AttributeWithValueCollectionLdap
{
    private readonly List<AttributeWithValueLdap> _attributes = [];
    private readonly List<AttributeWithValueLdap> _attributesObjectClass = [];
    private readonly string _filterObjectClass;

    public AttributeWithValueCollectionLdap(ObjectClassTypeLdap objectClassType = ObjectClassTypeLdap.User)
    {
        _attributesObjectClass.Add(new ObjectClassLdap(objectClassType).AttributeWithValues);
        if (objectClassType == ObjectClassTypeLdap.User)
        {
            _attributesObjectClass.Add(new ObjectClassLdap(ObjectClassTypeLdap.Computer, OperatorLdap.NotEqual)
                .AttributeWithValues);
        }

        _filterObjectClass = $"&{string.Join("", _attributesObjectClass.Select(a => a.AttributeWithValueString))}";
    }

    public void Add(AttributeTypeLdap attributeType, List<object?> values, OperatorLdap comparison = OperatorLdap.Equal)
    {
        AttributeWithValueLdap attributeWithValue = new(attributeType, values, comparison);
        if (string.IsNullOrEmpty(attributeWithValue.AttributeWithValueString)) return;
        _attributes.Add(attributeWithValue);
    }

    public void Add(AttributeWithValueLdap attributeWithValue)
    {
        _attributes.Add(attributeWithValue);
    }

    public void AddRange(IEnumerable<AttributeWithValueLdap> attributeWithValues)
    {
        _attributes.AddRange(attributeWithValues);
    }

    public string GetAndFilter()
    {
        return _attributes.Count == 0
            ? string.Empty
            : $"({_filterObjectClass}{string.Join("", _attributes.Select(a => a.AttributeWithValueString))})";
    }

    public string GetOrFilter()
    {
        return _attributes.Count == 0
            ? string.Empty
            : $"({_filterObjectClass}(|{string.Join("", _attributes.Select(a => a.AttributeWithValueString))}))";
    }

    public int Count => _attributes.Count;

    private static string JoinFilter(string[]? filters, bool isAnd = true)
    {
        if (filters is null || filters.Length == 0) return string.Empty;
        if (filters.Length == 1) return filters[0];
        var joinedFilters = string.Join("", filters.Select(f => $"({f})"));
        if (isAnd) return $"(&{joinedFilters})";
        else return $"(|{joinedFilters})";
    }

    public static string JoinFilter(AttributeWithValueCollectionLdap[]? attributeWithValueCollection,
        bool getAndFilterAttributeWithValueCollection = true, bool isAnd = true)
    {
        if (attributeWithValueCollection is null || attributeWithValueCollection.Length == 0) return string.Empty;
        if (attributeWithValueCollection.Length == 1)
        {
            if (getAndFilterAttributeWithValueCollection) return attributeWithValueCollection[0].GetAndFilter();
            else return attributeWithValueCollection[0].GetOrFilter();
        }

        if (getAndFilterAttributeWithValueCollection)
            return JoinFilter(attributeWithValueCollection.Select(a => a.GetAndFilter()).ToArray(), isAnd);
        else return JoinFilter(attributeWithValueCollection.Select(a => a.GetOrFilter()).ToArray(), isAnd);
    }
}