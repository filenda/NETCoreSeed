
using NETCoreSeed.Context.DataContext;

namespace NETCoreSeed.Context.EntityMapping
{
    internal class CheckInMap : DbEntityConfiguration<CoreDomain.Entities.CheckIn>
    {
        public override void Configure(EntityTypeBuilder<CoreDomain.Entities.CheckIn> entity)
        {
            // PK
            entity.HasKey(ck => ck.CheckInId);

            // Relationships
            entity
                .HasOne(ck => ck.Class)
                .WithMany(c => c.CheckIns)
                .HasForeignKey(ck => ck.ClassId);

            entity
                .HasOne(ck => ck.User)
                .WithMany(u => u.CheckIns)
                .HasForeignKey(ck => ck.UserId);
        }
    }
}