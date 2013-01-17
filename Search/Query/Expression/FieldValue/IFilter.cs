namespace Query.Expression.FieldValue
{
    public interface IFilter: IFieldValue
    {
        object GetFilter(string fieldName);
    }
}
