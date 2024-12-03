using Haihv.DatDai.Lib.Identity.Ldap.Enum;

namespace Haihv.DatDai.Lib.Identity.Ldap.Extension;


public class AttributeWithValueLdap(AttributeTypeLdap attributeType, List<object?> values, OperatorLdap operatorLdap = OperatorLdap.Equal)
{
    public AttributeTypeLdap AttributeType => attributeType;
    private string AttributeName => AttributeLdap.GetAttribute(attributeType);
    private List<object?> AttributeValues => values;
    public OperatorLdap Comparison => operatorLdap;
    public string AttributeWithValueString
    {
        get
        {
            if (AttributeValues.Count == 0) return string.Empty;
            List<string> searchByString = [];
            if (attributeType == AttributeTypeLdap.ObjectGuid)
            {
                foreach (var value in AttributeValues)
                {
                    if (!Guid.TryParse(value!.ToString(), out var guid)) continue;
                    var objectGuidBytes = guid.ToByteArray();
                    var octetString = objectGuidBytes.Aggregate(string.Empty, (current, b) => current + $@"\{b:X2}");
                    searchByString.Add($"({AttributeName}={octetString})");
                }
            }
            else
            {
                var comparisonString = operatorLdap switch
                {
                    OperatorLdap.GreaterThanOrEqual => ">=",
                    OperatorLdap.LessThanOrEqual => "<=",
                    _ => "=",
                };
                foreach (var value in AttributeValues.OfType<object>())
                {
                    var formattedValue = value is DateTime dateTimeValue
                        ? dateTimeValue.ToString("yyyyMMddHHmmss.0Z")
                        : value.ToString();

                    var comparisonOperator = operatorLdap != OperatorLdap.NotEqual
                        ? comparisonString
                        : "=";

                    var prefix = operatorLdap != OperatorLdap.NotEqual
                        ? string.Empty
                        : "!";
                    if (string.IsNullOrEmpty(formattedValue)) continue;
                    searchByString.Add(string.IsNullOrEmpty(prefix)
                        ? $"({AttributeName}{comparisonOperator}{formattedValue})"
                        : $"({prefix}({AttributeName}{comparisonOperator}{formattedValue}))");
                }
            }
            if (searchByString.Count == 0) return string.Empty;
            return searchByString.Count == 1 ? searchByString[0] : $"(|{string.Join("", searchByString)})";
        }
    }
}