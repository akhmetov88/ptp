using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TradingPlatform.Data;
using System.Data.Entity.Validation;

namespace TradingPlatform.Models
{

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.UserContragents = new HashSet<Contragent>();
            this.InboxNotifications = new HashSet<Notification>();
            this.OutboxNotifications = new HashSet<Notification>();
            this.ApproovedContragents = new HashSet<Contragent>();
            this.CreatedContragents = new HashSet<Contragent>();
            this.ApprovedFiles = new HashSet<File>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim("UserRegisterName", this.RegisterName ?? UserName));

            userIdentity.AddClaim(new Claim("UserGroup", this.GroupId?.ToString() ?? ""));

            userIdentity.AddClaim(this.GroupId.HasValue
                ? new Claim("UserContragents",
                    String.Join(",",
                        this.Group.Contragents.ToList().Where(c => c.IsApproved).Select(c => c.Id)?.ToList()))
                : new Claim("UserContragents", ""));


            // Add custom user claims here
            return userIdentity;
        }
      //  [LDisplayName("displayName", "BaseEntityUserName", "Ким створено", "Кем создано")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RegisterName
        {
            get
            {
                return String.IsNullOrWhiteSpace($"{UserInfo?.Name} {UserInfo?.Patronymyc} {UserInfo?.Surname}") ? Email : $"{UserInfo?.Name} {UserInfo?.Patronymyc} {UserInfo?.Surname}";
            }
            set
            { }
        }
        public bool AllowPromoEmails { get; set; }
        public bool AllowTradeEmails { get; set; }
        public bool IpRestricted { get; set; }
        public string AllowedIp { get; set; }
        public string Locale { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime LastLoggedIn { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Registered { get; set; }
        public int? GroupId { get; set; }
        public virtual Group Group { get; set; }

        public virtual UserInfo UserInfo { get; set; }

        public virtual ICollection<Contragent> UserContragents { get; set; }

        public virtual ICollection<Contragent> ApproovedContragents { get; set; }

        public virtual ICollection<Contragent> CreatedContragents { get; set; }

        public virtual ICollection<File> ApprovedFiles { get; set; }

        public virtual ICollection<Notification> InboxNotifications { get; set; }

        public virtual ICollection<Notification> OutboxNotifications { get; set; }

        public bool IsDebug { get; set; }
        public bool IsAcceptedOffert { get; set; }
    }

    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
    public class ApplicationUserClaim : IdentityUserClaim { }

    public class ApplicationUserLogin : IdentityUserLogin
    {
        public ApplicationUserLogin()
        {
            LoginProvider = "";
            ProviderKey = "";
            UserId = "";
        }
    }

    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUserRole()
        {

        }
       // public virtual ICollection<ApplicationUser> RoleUsers { get; set; }
    }
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser>, IDisposable
    {
        public ApplicationUserStore(ApplicationDbContext context) : base(context)
        {

        }
    }

    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>
    {
        public ApplicationRoleStore(ApplicationDbContext context) : base(context) { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public virtual DbSet<BankBill> BankBills { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Contragent> Contragents { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Content> ContentPages { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<Trade> Trades { get; set; }
        public virtual DbSet<Bet> Bets { get; set; }
        public virtual DbSet<Lot> Lots { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<TradeBill> TradeBills { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Lang> Langs { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<Catalog> Catalogs { get; set; }
        public virtual DbSet<HelpGroup> HelpGroups { get; set; }
        public virtual DbSet<Help> Helps { get; set; }
        public virtual DbSet<Push> Pushes { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<QuotationType> QuotationTypes { get; set; }
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<New> News { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().HasKey(c => c.Id).ToTable("Users").Property(p => p.Id);
            modelBuilder.Entity<ApplicationRole>().HasKey<string>(r => r.Id).ToTable("Roles");
        //    modelBuilder.Entity<ApplicationUserRole>().HasMany(f=>f.RoleUsers)
            modelBuilder.Entity<ApplicationUserClaim>().HasKey(c => c.Id).ToTable("UserClaims");
            modelBuilder.Entity<ApplicationUserLogin>().HasKey<string>(l => l.UserId).ToTable("UserLogins").HasKey(p => new { p.LoginProvider, p.ProviderKey, p.UserId });
            modelBuilder.Entity<ApplicationUserRole>().HasKey(r => new { r.RoleId, r.UserId }).ToTable("UserRoles");

            modelBuilder.Entity<Trade>()
                .HasMany(c => c.Orders)
                .WithRequired(c => c.Trade)
                .HasForeignKey(c => c.TradeId);

            modelBuilder.Entity<Trade>()
              .HasMany(c => c.News)
              .WithOptional(c => c.Trade)
              .HasForeignKey(c => c.TradeId);

            modelBuilder.Entity<Quotation>()
                .HasRequired(c => c.QuotationType)
                .WithMany(c => c.Quotations)
                .HasForeignKey(c => c.QuotationTypeId);

            modelBuilder.Entity<Catalog>()
                    .HasMany(c => c.Dependencies)
                    .WithOptional(c => c.Parent)
                    .HasForeignKey(c => c.ParentId);

            modelBuilder.Entity<Trade>()
                .HasMany(c => c.Lots)
                .WithRequired(c => c.Trade)
                .HasForeignKey(c => c.TradeId);

            modelBuilder.Entity<Lot>().HasOptional(f => f.Buyer);

            modelBuilder.Entity<Trade>()
                .HasOptional(c => c.AcceptedByUser);

            modelBuilder.Entity<Trade>()
                .HasOptional(c => c.ApprovedByUser);

            modelBuilder.Entity<Push>()
                .HasOptional(c => c.User);

            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(c => c.UserInfo)
                .WithRequired(c => c.User);

            modelBuilder.Entity<Group>()
                .HasMany(c => c.Contragents)
                .WithRequired(c => c.Group)
                .HasForeignKey(c => c.GroupId);

            modelBuilder.Entity<Group>()
                .HasMany(c => c.Users)
                .WithOptional(c => c.Group)
                .HasForeignKey(c => c.GroupId);

            modelBuilder.Entity<Trade>().HasMany(c => c.Bets)
                .WithRequired(c => c.Trade)
                .HasForeignKey(c => c.TradeId);

            modelBuilder.Entity<Contragent>()
                .HasMany(c => c.Bets)
                .WithRequired(c => c.Buyer)
                .HasForeignKey(c => c.BuyerId);


            modelBuilder.Entity<Notification>()
                .HasOptional(c => c.FromUser)
                .WithMany(c => c.OutboxNotifications)
                .HasForeignKey(m => m.FromUserId);

            modelBuilder.Entity<Notification>()
                .HasOptional(c => c.ToUser)
                .WithMany(c => c.InboxNotifications)
                .HasForeignKey(m => m.ToUserId);


            modelBuilder.Entity<Contragent>()
                .HasRequired(c => c.CreatedByUser)
                .WithMany(c => c.CreatedContragents)
                .HasForeignKey(c => c.CreatedByUserId);

            modelBuilder.Entity<Contragent>()
               .HasOptional(c => c.ApprovedByUser)
               .WithMany(c => c.ApproovedContragents)
               .HasForeignKey(c => c.ApprovedByUserId);

            modelBuilder.Entity<File>()
                .HasOptional(f => f.ApprovedByUser)
                .WithMany(c => c.ApprovedFiles)
                .HasForeignKey(c => c.ApprovedByUserId);


            modelBuilder.Entity<ApplicationUser>().
                HasMany(c => c.UserContragents).
                WithMany(c => c.ContragentUsers).
                Map(m => m.ToTable("UserContragents"));


            modelBuilder.Entity<Contragent>()
                .HasMany(c => c.TradesListSeller)
                .WithRequired(c => c.Seller)
                .HasForeignKey(c => c.SellerId);

            modelBuilder.Entity<Contragent>()
                .HasMany(c => c.TradesListBuyer)
                .WithMany(c => c.Buyers)
                .Map(c => c.ToTable("BuyerTrades"));

            modelBuilder.Entity<Feedback>()
                .HasOptional(c => c.User);


            modelBuilder.Entity<Contragent>()
                .HasMany(c => c.Documents)
                .WithRequired(c => c.Contragent)
                .HasForeignKey(c => c.ContragentId);

            modelBuilder.Entity<Contragent>()
                .HasMany(c => c.InBills)
                .WithRequired(c => c.Buyer)
                .HasForeignKey(c => c.ToContragentId);

            modelBuilder.Entity<Contragent>()
              .HasMany(c => c.OutBills)
              .WithRequired(c => c.Seller)
              .HasForeignKey(c => c.FromContragentId);

            modelBuilder.Entity<Bet>()
                .HasOptional(c => c.Rebetter);

            modelBuilder.Entity<Contragent>()
                .HasMany(c => c.SellerContracts)
                .WithRequired(c => c.FromContragent)
                .HasForeignKey(c => c.FromContragentId);

            modelBuilder.Entity<Contragent>()
               .HasMany(c => c.BuyerContracts)
               .WithRequired(c => c.ToContragent)
               .HasForeignKey(c => c.ToContragentId);

            modelBuilder.Entity<TradeBill>()
                .HasRequired(c => c.Contractt)
                .WithMany(c => c.TradeBills)
                .HasForeignKey(c => c.ContracttId);

            modelBuilder.Entity<File>()
                .HasOptional(f => f.FileType);

            modelBuilder.Entity<Bet>()
                .HasOptional(c => c.Lot)
                .WithMany(c => c.Bets)
                .HasForeignKey(f => f.LotId);

            modelBuilder.Entity<Lot>()
                .HasMany(c => c.Bets)
                .WithOptional(c => c.Lot)
                .HasForeignKey(f => f.LotId);

        }

        private static string connectionStrings
        {
            get
            {
                return "DataEntities";

            }
        }

        public ApplicationDbContext() : base(connectionStrings)
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
            //t.OnModelCreating(new DbModelBuilder());
            //return t;
        }

        public IEnumerable<Trade> TradesApproved(int? count = null)
        {
            return Trades.Where(c => !c.IsOffer && !c.IsOrder).OrderByDescending(c => c.DateBegin).ThenByDescending(c => c.Id).ToList().Where(c => c.IsFinallyApproved.Value).Take(count ?? Trades.Count());
        }
        public IEnumerable<Trade> TradesNotApproved()
        {
            return Trades.AsEnumerable().Where(c => !c.IsFinallyApproved.Value && !c.IsOffer && !c.IsOrder).ToList();
        }



        #region Utilities

        /// <summary>
        /// Get full error
        /// </summary>
        /// <param name="exc">Exception</param>
        /// <returns>Error</returns>
        protected string GetFullErrorText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var validationErrors in exc.EntityValidationErrors)
                foreach (var error in validationErrors.ValidationErrors)
                    msg += string.Format("Property: {0} Error: {1}", error.PropertyName, error.ErrorMessage) + Environment.NewLine;
            return msg;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Insert<TEntity>(TEntity entity, string userId = null) where TEntity : BaseEntity
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                entity.Created = DateTime.UtcNow;
                entity.CreatedByUserId = userId;

                this.Entry(entity).State = EntityState.Added;
                this.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert<T>(IEnumerable<T> entities, string userId = null) where T : BaseEntity
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                {
                    entity.Created = DateTime.UtcNow;
                    entity.CreatedByUserId = userId;
                    this.Entry(entity).State = EntityState.Added;
                }

                this.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void UpdateEntity<T>(T entity, string userId = null) where T : BaseEntity
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                entity.Updated = DateTime.UtcNow;
                entity.UpdatedByUserId = userId;
                this.Entry(entity).State = EntityState.Modified;

                this.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task UpdateEntityAsync<T>(T entity, string userId = null) where T : BaseEntity
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                entity.Updated = DateTime.UtcNow;
                entity.UpdatedByUserId = userId;
                this.Entry(entity).State = EntityState.Modified;

                await this.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete<T>(T entity) where T : BaseEntity
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entry(entity).State = EntityState.Deleted;
                SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    this.Entry(entity).State = EntityState.Deleted;

                this.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        #endregion


    }
}