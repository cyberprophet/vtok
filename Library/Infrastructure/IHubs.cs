namespace ShareInvest.Infrastructure;

public interface IHubs
{
    Task GatherCluesToPrioritize(int count);

    Task InstructToRenewAssetStatus(string account);

    Task UpdateTheStatusOfAssets<T>(T asset) where T : class;

    Task UpdateTheStatusOfBalances<T>(T balance) where T : class;

    Task AddToGroupAsync(string groupName);

    Task TransmitConclusionInformation(string key, string data);

    Task GetAssetStatusByDate<T>(T enumerable);
}