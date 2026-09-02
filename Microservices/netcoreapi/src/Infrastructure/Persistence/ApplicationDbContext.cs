//using Application.Common.Interfaces;
//using Domain.Common;
//using Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Infrastructure.Persistence
//{
//	public class ApplicationDbContext : DbContext, IApplicationDbContext // ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
//	{
//		private readonly ICurrentUserService _currentUserService;
//		private readonly IDateTime _dateTime;
//		private readonly IDomainEventService _domainEventService;

//		public ApplicationDbContext(
//			DbContextOptions options,
//			// IOptions<OperationalStoreOptions> operationalStoreOptions,
//			ICurrentUserService currentUserService,
//			IDomainEventService domainEventService,
//		  //  IDateTime dateTime) : base(options, operationalStoreOptions)
//		  IDateTime dateTime) : base(options)
//		{
//			_currentUserService = currentUserService;
//			_domainEventService = domainEventService;
//			_dateTime = dateTime;
//		}

//		// public DbSet<TodoItem> TodoItems { get; set; }

//		//  public DbSet<TodoList> TodoLists { get; set; }
//		public DbSet<AcmeProduct> AcmeProduct { get; set; }

//		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
//		{
//			foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
//			{
//				switch (entry.State)
//				{
//					case EntityState.Added:
//						entry.Entity.CreatedBy = _currentUserService.UserId;
//						entry.Entity.CreatedDateTime = _dateTime.Now;
//						break;

//					case EntityState.Modified:
//						entry.Entity.UpdatedBy = _currentUserService.UserId;
//						entry.Entity.UpdatedDateTime = _dateTime.Now;
//						break;
//				}
//			}

//			var result = await base.SaveChangesAsync(cancellationToken);

//			//await DispatchEvents();

//			return result;
//		}

//		protected override void OnModelCreating(ModelBuilder builder)
//		{
//			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

//			base.OnModelCreating(builder);
//		}

//		//private async Task DispatchEvents()
//		//{
//		//	while (true)
//		//	{
//		//		var domainEventEntity = ChangeTracker.Entries<IHasDomainEvent>()
//		//			.Select(x => x.Entity.DomainEvents)
//		//			.SelectMany(x => x)
//		//			.Where(domainEvent => !domainEvent.IsPublished)
//		//			.FirstOrDefault();
//		//		if (domainEventEntity == null) break;

//		//		domainEventEntity.IsPublished = true;
//		//		await _domainEventService.Publish(domainEventEntity);
//		//	}
//		//}
//	}
//}
