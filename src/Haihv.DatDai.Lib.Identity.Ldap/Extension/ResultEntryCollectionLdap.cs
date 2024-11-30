using System.DirectoryServices.Protocols;
using Haihv.DatDai.Lib.Identity.Ldap.Enum;

namespace Haihv.DatDai.Lib.Identity.Ldap.Extension;

public class ResultEntryCollectionLdap(ILdapContext ldapContext)
{
    private readonly LdapConnection _connectionLdap = ldapContext.Connection;
    private readonly LdapConnectionInfo _ldapConnectionInfo = ldapContext.LdapConnectionInfo;
    private SearchResultEntryCollection? Get(string filter, string[] attributesToReturn)
    {
        using var ldapConnection = _connectionLdap;
        SearchRequest searchRequest = new(_ldapConnectionInfo.SearchBase, filter, SearchScope.Subtree, attributesToReturn);
        var searchResponse = ldapConnection.SendRequest(searchRequest);
        if (searchResponse is SearchResponse searchRes)
            return searchRes.Entries;
        else
            return null;
    }

    private async Task<SearchResultEntryCollection?> GetAsync(string filter, string[] attributesToReturn)
    {
        return await Task.Run(() => Get(filter, attributesToReturn));
    }

    private SearchResultEntryCollection? Get(string filter, AttributeTypeLdap[] attributesToReturn)
    {
        return Get(filter, AttributeLdap.GetAttributes(attributesToReturn));
    }
    public async Task<SearchResultEntryCollection?> GetAsync(string filter, AttributeTypeLdap[] attributesToReturn)
    {
        return await GetAsync(filter, AttributeLdap.GetAttributes(attributesToReturn));
    }

    private SearchResultEntryCollection? Get(AttributeWithValueCollectionLdap filterCollection, AttributeTypeLdap[] attributesToReturn, bool isAnd = true)
    {
        // Dựa vào tham số isAnd, tạo điều kiện filter dựa trên cách kết hợp các thuộc tính của filterCollection
        // Nếu isAnd = true, thì dùng cách kết hợp AND, ngược lại dùng cách kết hợp OR
        var filter = isAnd ? filterCollection.GetAndFilter() : filterCollection.GetOrFilter();
        return Get(filter, attributesToReturn);
    }
    public async Task<SearchResultEntryCollection?> GetAsync(AttributeWithValueCollectionLdap filterCollection, AttributeTypeLdap[] attributesToReturn, bool isAnd = true)
    {
        return await Task.Run(() => Get(filterCollection, attributesToReturn, isAnd));
    }

    private object? FirstOrDefaultValue(AttributeWithValueCollectionLdap filterCollection, AttributeTypeLdap attributeToReturn, bool isAnd = true)
    {
        var searchResultEntryCollection = Get(filterCollection, [attributeToReturn], isAnd);
        if (searchResultEntryCollection is null || searchResultEntryCollection.Count == 0)
        {
            return null;
        }
        return searchResultEntryCollection[0].Attributes[AttributeLdap.GetAttribute(attributeToReturn)]?[0];
    }
    public async Task<object?> FirstOrDefaultValueAsync(AttributeWithValueCollectionLdap filterCollection, AttributeTypeLdap attributeToReturn, bool isAnd = true)
    {
        return await Task.Run(() => FirstOrDefaultValue(filterCollection, attributeToReturn, isAnd));
    }

    private string StringFirstValue(AttributeWithValueCollectionLdap filterCollection, AttributeTypeLdap attributeToReturn, bool isAnd = true)
    {
        var value = FirstOrDefaultValue(filterCollection, attributeToReturn, isAnd);
        return value?.ToString() ?? string.Empty;
    }
    public async Task<string> StringFirstValueAsync(AttributeWithValueCollectionLdap filterCollection, AttributeTypeLdap attributeToReturn, bool isAnd = true)
    {
        return await Task.Run(() => StringFirstValue(filterCollection, attributeToReturn, isAnd));
    }
}