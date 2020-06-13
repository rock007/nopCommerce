using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.SMS.Aliyun.Domain;

namespace Nop.Plugin.SMS.Aliyun.Data
{

    //public partial class SmsTemplateMap : NopEntityTypeConfiguration<SmsTemplate>
    //{
    //    public SmsTemplateMap()
    //    {
    //        this.ToTable("SmsTemplate");
    //        this.HasKey(x => x.Id);

    //        this.Property(x => x.Body).HasMaxLength(4000);
    //    }
    //}

    public partial class SmsTemplateMap : NopEntityTypeConfiguration<SmsTemplate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<SmsTemplate> builder)
        {
            builder.ToTable(nameof(QueuedSms));
            builder.HasKey(record => record.Id);

            builder.Property(record => record.Body).HasMaxLength(2000);
            builder.Property(record => record.Subject).HasMaxLength(255);
            builder.Property(record => record.TemplateCode).HasMaxLength(63);
            builder.Property(record => record.SystemName).HasMaxLength(255);

        }

        #endregion
    }
}
