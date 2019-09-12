using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicDataServiceTests.TestingHelpers
{
    public class Item
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ItemType Type { get; set; }
    }
    public class ItemType
    {
        public ItemType()
        {
            Items = new HashSet<Item>();
        }
        public int ID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }


    class MockContext : DbContext
    {

        public MockContext()
        {
            this.Types = new MockItemTypeDbSet();
            Items = new MockItemDbSet(Types);
        }
        
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemType> Types { get; set; }
    }

    class MockItemDbSet : MockDbSet<Item>
    {
        public MockItemDbSet(DbSet<ItemType> types)
        {
            this.Add(new Item()
            {
                ID = 1,
                Name = "Bolt",
                Type = types.First()
            });
            this.Add(new Item()
            {
                ID = 1,
                Name = "Washer",
                Type = types.Skip(1).First()
            });
        }

        public override Item Find(params object[] keyValues)
        {
            return this.SingleOrDefault(m => m.ID == (int)keyValues.Single());
        }
    }

    class MockItemTypeDbSet : MockDbSet<ItemType>
    {
        public MockItemTypeDbSet()
        {
            this.Add(new ItemType()
            {
                ID = 1,
                Name = "Metal"
            });
            this.Add(new ItemType()
            {
                ID = 1,
                Name = "Rubber"
            });
        }

        public override ItemType Find(params object[] keyValues)
        {
            return this.SingleOrDefault(m => m.ID == (int)keyValues.Single());
        }
    }

    public class MockDbSet<T> : DbSet<T>, IQueryable, IEnumerable<T>, IDbAsyncQueryProvider
        where T : class
    {
        ObservableCollection<T> _data;
        IQueryable _query;

        public MockDbSet()
        {
            _data = new ObservableCollection<T>();
            _query = _data.AsQueryable();
        }

        public override T Add(T item)
        {
            _data.Add(item);
            return item;
        }

        public override T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public override T Attach(T item)
        {
            _data.Add(item);
            return item;
        }

        public override T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return Task.FromResult(_data.FirstOrDefault());
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<T> Local
        {
            get { return new ObservableCollection<T>(_data); }
        }

        Type IQueryable.ElementType
        {
            get { return _query.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _query.Provider; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
