namespace NETCoreSeed.Shared.Specification.Interface
{
    public interface ISpecification<in TEntity>
    {
        bool IsSatisfiedBy(TEntity entity);
    }
}