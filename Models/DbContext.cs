using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebCrawler.Models
{
    public partial class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext()
        {
        }

        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DataType> DataType { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Link> Link { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<PageData> PageData { get; set; }
        public virtual DbSet<PageType> PageType { get; set; }
        public virtual DbSet<Site> Site { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("data_type", "crawldb");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("character varying(20)")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("image", "crawldb");

                entity.HasIndex(e => e.PageId)
                    .HasName("idx_image_page_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('crawldb.image_id_seq'::regclass)");

                entity.Property(e => e.AccessedTime).HasColumnName("accessed_time");

                entity.Property(e => e.ContentType)
                    .HasColumnName("content_type")
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.Data).HasColumnName("data");

                entity.Property(e => e.Filename)
                    .HasColumnName("filename")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.PageId).HasColumnName("page_id");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.Image)
                    .HasForeignKey(d => d.PageId)
                    .HasConstraintName("fk_image_page_data");
            });

            modelBuilder.Entity<Link>(entity =>
            {
                entity.HasKey(e => new { e.FromPage, e.ToPage });

                entity.ToTable("link", "crawldb");

                entity.HasIndex(e => e.FromPage)
                    .HasName("idx_link_from_page");

                entity.HasIndex(e => e.ToPage)
                    .HasName("idx_link_to_page");

                entity.Property(e => e.FromPage).HasColumnName("from_page");

                entity.Property(e => e.ToPage).HasColumnName("to_page");

                entity.HasOne(d => d.FromPageNavigation)
                    .WithMany(p => p.LinkFromPageNavigation)
                    .HasForeignKey(d => d.FromPage)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_link_page");

                entity.HasOne(d => d.ToPageNavigation)
                    .WithMany(p => p.LinkToPageNavigation)
                    .HasForeignKey(d => d.ToPage)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_link_page_1");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("page", "crawldb");

                entity.HasIndex(e => e.PageTypeCode)
                    .HasName("idx_page_page_type_code");

                entity.HasIndex(e => e.SiteId)
                    .HasName("idx_page_site_id");

                entity.HasIndex(e => e.Url)
                    .HasName("unq_url_idx")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('crawldb.page_id_seq'::regclass)");

                entity.Property(e => e.AccessedTime).HasColumnName("accessed_time");

                entity.Property(e => e.HtmlContent).HasColumnName("html_content");

                entity.Property(e => e.HttpStatusCode).HasColumnName("http_status_code");

                entity.Property(e => e.PageTypeCode)
                    .HasColumnName("page_type_code")
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.SiteId).HasColumnName("site_id");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasColumnType("character varying(3000)");

                entity.HasOne(d => d.PageTypeCodeNavigation)
                    .WithMany(p => p.Page)
                    .HasForeignKey(d => d.PageTypeCode)
                    .HasConstraintName("fk_page_page_type");

                entity.HasOne(d => d.Site)
                    .WithMany(p => p.Page)
                    .HasForeignKey(d => d.SiteId)
                    .HasConstraintName("fk_page_site");
            });

            modelBuilder.Entity<PageData>(entity =>
            {
                entity.ToTable("page_data", "crawldb");

                entity.HasIndex(e => e.DataTypeCode)
                    .HasName("idx_page_data_data_type_code");

                entity.HasIndex(e => e.PageId)
                    .HasName("idx_page_data_page_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('crawldb.page_data_id_seq'::regclass)");

                entity.Property(e => e.Data).HasColumnName("data");

                entity.Property(e => e.DataTypeCode)
                    .HasColumnName("data_type_code")
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.PageId).HasColumnName("page_id");

                entity.HasOne(d => d.DataTypeCodeNavigation)
                    .WithMany(p => p.PageData)
                    .HasForeignKey(d => d.DataTypeCode)
                    .HasConstraintName("fk_page_data_data_type");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.PageData)
                    .HasForeignKey(d => d.PageId)
                    .HasConstraintName("fk_page_data_page");
            });

            modelBuilder.Entity<PageType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("page_type", "crawldb");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("character varying(20)")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Site>(entity =>
            {
                entity.ToTable("site", "crawldb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('crawldb.site_id_seq'::regclass)");

                entity.Property(e => e.Domain)
                    .HasColumnName("domain")
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.RobotsContent).HasColumnName("robots_content");

                entity.Property(e => e.SitemapContent).HasColumnName("sitemap_content");
            });

            modelBuilder.HasSequence<int>("image_id_seq");

            modelBuilder.HasSequence<int>("page_data_id_seq");

            modelBuilder.HasSequence<int>("page_id_seq");

            modelBuilder.HasSequence<int>("site_id_seq");
        }
    }
}
