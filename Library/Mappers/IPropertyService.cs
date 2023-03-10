namespace ShareInvest.Mappers;

public interface IPropertyService
{
    void SetValuesOfColumn<T>(T tuple, T param) where T : class;
}