namespace ShareInvest.Mappers;

public interface ISecuritiesMapper<T>
{
    event EventHandler<T> Send;
}