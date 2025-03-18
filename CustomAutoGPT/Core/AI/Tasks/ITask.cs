namespace AutoGPTDotNet.Core.Tasks
{
    public interface ITask
    {
        string Description { get; }
        void Execute();
    }
}