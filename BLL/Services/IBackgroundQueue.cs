namespace BLL.Services
{
    public interface IBackgroundQueue<T>
    {
        void Enqueue(T item);
        T Dequeue();
    }
}
