namespace StartspelerAPI.Data.Repository
{
    public interface IGenericRepository <TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
	    Task<TEntity> GetByIdAsync(int id);
	    Task AddAsync(TEntity entity);
        void Update(TEntity entity);
	    void Delete(TEntity entity);
    }
}
