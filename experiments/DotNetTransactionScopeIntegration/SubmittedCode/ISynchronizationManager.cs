

public interface ISynchronizationManager {

    void RegisterSynchronization(ISynchronization synchronization);

    void CreateSynchronizationBlock();

    void PerformSynchronizations(bool commited);
}