using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Channels;
using static System.Net.Mime.MediaTypeNames;


// 판서 복습

// 1. 게임슬릇에 아이템 장착 - 해제하는 시스템
/*
Slot<Weapon> weaponSlot = new Slot<Weapon>("무기");
weaponSlot.Equip(new Weapon { Name = "철검", Damage = 25 });
Console.WriteLine(weaponSlot);

weaponSlot.Unequip();
Console.WriteLine(weaponSlot);

Slot<Potion> potionSlot = new Slot<Potion>("포션");
potionSlot.Equip(new Potion { Name = "체력 물약", HealAmount = 50 });
Console.WriteLine(potionSlot);
// 아이템 클래스 (무기, 포션)
public class Weapon
{
    public string Name { get; set; }
    public int Damage { get; set; }

    public override string ToString() => $"{Name}(공격력: {Damage})";
}

public class Potion
{
    public string Name { get; set; }
    public int HealAmount { get; set; }

    public override string ToString() => $"{Name}(회복량: {HealAmount})";
}
// 게임 슬릇 클래스
public class Slot<T>
{
    private string _name;
    private T _item;
    private bool _hasItem;

    public Slot(string name)
    {
        _name = name;
        _hasItem = false;
    }

    public void Equip(T item)
    {
        _item = item;
        _hasItem = true;
        Console.WriteLine($"[{_name}] {item} 장착!");
    }

    public T Unequip()
    {
        if (!_hasItem)
        {
            Console.WriteLine($"[{_name}] 비어 있음");
            return default(T);
        }
        T removed = _item;    //_item은 인스턴스임 , removed 변수에 따로 넣어주는 이유는? 다른 곳으로 옮겨두거나 저장해두고 어디선가 다시 사용하기위해?
        _item = default(T);
        _hasItem = false;
        Console.WriteLine($"[{_name}] {removed} 해제!");
        return removed;
    }

    public override string ToString()
    {
        return _hasItem ? $"[{_name}] -> {_item}" : $"[{_name}] -> (비어 있음)";
    }
}


//2. 필드, 속성, 메서드에서 타입 매개변수 사용
Container<string> stringContainer = new Container<string>("Hello");
stringContainer.Display();

Container<double> doubleContainer = new Container<double>(3.14);
doubleContainer.Display();

Container<bool> boolContainer = new Container<bool>();
boolContainer.Display();
public class Container<T>
{
    private T _data;

    public Container()
    {
        _data = default(T);
    }
    public Container(T data)
    {
        _data = data;
    }

    public T Data
    {
        get { return _data; }
        set { _data = value; }
    }

    public void Display()
    {
        Console.WriteLine($"저장된 값: {_data}");
    }
}


//3. 두 개 이상의 타입 매개변수 사용
var record1 = new GameRecord<string, int>("용사", 1500);
var record2 = new GameRecord<string, double>("용사", 2350.5);

record1.Display();
record2.Display();

Console.WriteLine();
Console.WriteLine(record1);
Console.WriteLine(record2);

public class GameRecord<TPlayer, TScore>
{
    public TPlayer Player { get; set; }
    public TScore Score { get; set; }

    public GameRecord(TPlayer player, TScore score)
    {
        Player = player; 
        Score = score; 
    }

    public void Display()
    {
        Console.WriteLine($"플레이어: {Player}, 점수: {Score}");
    }
    public override string ToString()
    {
        return $"[{Player}] {Score}점";
    }
}


//4. 제네릭 인터페이스 구현
IStorage<string> stringStorage = new SimpleStorage<string>();  // 인터페이스로 선언하면 나중에 fastStorage 등 다른 클래스를 만들었을때도 용이하게 사용할 수 있다
stringStorage.Store("중요한 데이터");
Console.WriteLine($"꺼낸 값: {stringStorage.Retrieve()}");

IStorage<int> intStorage = new SimpleStorage<int>();
intStorage.Store(12345);
Console.WriteLine($"꺼낸 값: {intStorage.Retrieve()}");
public interface IStorage<T>
{
    void Store(T item);
    T Retrieve();
    bool HasItem {  get; }
}
public class SimpleStorage<T> : IStorage<T>
{
    private T _storedItem;
    private bool _hasItem = false;

    public bool HasItem => _hasItem;  //프로퍼티

    public void Store(T item)
    {
        _storedItem = item; 
        _hasItem = true;
        Console.WriteLine($"저장 완료: {item}");
    }

    public T Retrieve()
    {
        if (!_hasItem)
        {
            throw new InvalidOperationException("저장된 아이템이 없습니다.");
        }
        _hasItem = false ;
        return _storedItem;
    }
}


//5. 닫힌 제네릭 타입으로 구현
Person p1 = new Person { Name = "홍길동", Age = 25 };
Person p2 = new Person { Name = "김철수", Age = 30 };

int result = p1.CompareTo(p2);
Console.WriteLine($"{p1.Name}과 {p2.Name} 비교: {result}");

public interface IComparable<T>
{
    int CompareTo(T other);
}

public class Person : IComparable<Person>
{
    public string Name { get; set; }
    public int Age { get; set; }

    public int CompareTo(Person other)
    {
        return this.Age.CompareTo(other.Age);
    }
}


//6. struct 제약 조건  (값 타입 전용 게임 스탯 클래스)
var hp = new Stat<int>("체력", 100);
Console.WriteLine(hp);
Console.WriteLine($"기본값 여부: {hp.IsDefault()}");

hp.Value = 0;
Console.WriteLine($"기본값 여부: {hp.IsDefault()}");

var critical = new Stat<float>("치명타 확률", 15.5f);
Console.WriteLine(critical);

public class Stat<T> where T : struct
{
    private string _name;
    public T Value { get; set; }

    public Stat(string name, T value)
    {
        _name = name;
        Value = value;
    }

    public bool IsDefault()
    {
        return Value.Equals(default(T));
    }

    public override string ToString()
    {
        return $"[{_name}] {Value}";
    }
}


//7. class 제약 조건
ReferenceContainer<string> strContainer = new ReferenceContainer<string>();
Console.WriteLine($"null 여부: {strContainer.IsNull()}");

strContainer.Value = "Hello";
Console.WriteLine($"null 여부: {strContainer.IsNull()}");
public class ReferenceContainer<T> where T : class
{
    public T Value { get; set; }

    public bool IsNull()
    {
        return Value == null;
    }
}


//8. new() 제약 조건
Factory<Product> factory = new Factory<Product>();

Product product = factory.Create();
Console.WriteLine($"생성된 상품: {product.Name}");

Product[] products = factory.CreateArray(3);
Console.WriteLine($"배열 크기: {products.Length}");
public class Factory<T> where T : new()
{
    public T Create()   //인스턴스 생성하는 메서드
    {
        return new T();
    }

    public T[] CreateArray(int count) //베열 생성하는메서드
    {
        T[] array = new T[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = new T();  //배열에 인스턴스를 담아준다
        }
        return array;
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "새 상품";
}


// 9. 인터페이스 제약 조건   - 순위 업데이트 기능
LeaderBoard<int> scoreBoard = new LeaderBoard<int>(3);
scoreBoard.Submit(750);
scoreBoard.Submit(300);
scoreBoard.Submit(900);
scoreBoard.Submit(500);

scoreBoard.PrintRanking("점수 랭킹");
public class LeaderBoard<T> where T : IComparable<T>
{
    private List<T> _entries = new List<T>();
    private int _maxSize;

    public LeaderBoard(int maxSize)
    {
        _maxSize = maxSize; 
    }

    public void Submit(T entry)
    {
        _entries.Add(entry);

        for (int i = 0; i < _entries.Count - 1; i++)  //내림차순 정렬
        {
            for (int j = i+1; j < _entries.Count; j++)
            {
                if (_entries[i].CompareTo(_entries[j]) < 0)
                {
                    T temp = _entries[i];
                    _entries[i] = _entries[j];
                    _entries[j] = temp;
                }
            }
        }

        if (_entries.Count > _maxSize)
        {
            _entries.RemoveAt(_entries.Count - 1);  //인덱스넘버라서 -1
        }
    }

    public void PrintRanking(string title)
    {
        Console.WriteLine($"=== {title} ===");
        for (int i = 0; i < _entries.Count; i++)
        {
            Console.WriteLine($"   {i+1}위: {_entries[i]}");
        }
    }
}


// 10.
EntityManager<Player> playerManager = new EntityManager<Player>();
EntityManager<Monster> monsterManager = new EntityManager<Monster>();

playerManager.Add(new Player { Id = 1, Name = "홍길동" });
playerManager.Add(new Player { Id = 2, Name = "김철수" });
monsterManager.Add(new Monster { Id = 1, Level = 5 });

var found = playerManager.FindById(1);
Console.WriteLine($"찾은 플레이어: {found.Name}");
public class Entity
{
    public int Id { get; set; }
}
public class Player : Entity
{
    public string Name { get; set; }
}

class Monster : Entity
{
    public int Level { get; set; }
}

public class EntityManager<T> where T : Entity
{
    private List<T> _entities = new List<T>();

    public void Add(T entity)
    {
        _entities.Add(entity);
        Console.WriteLine($"엔티티 추가됨 (ID: {entity.Id})");
    }

    public T FindById(int id)
    {
        foreach (var entity in _entities)
        {
            if (entity.Id == id)
            {
                return entity;
            }
        }
        return null;
    }
}


// 11. 복합 제약 조건 - 오브젝트 풀링, new 키워드로 객체를 새로 만들고 버리는 대신
//다 쓴 객체를 보관함에 넣어두었다가 다시 꺼내쓰는구조
Pool<Product> pool = new Pool<Product>();  // 클래스 객체 생성 (안에 prodcut가 들어있는 큐 객체)

Product item1 = pool.Get(); //새로생성
Console.WriteLine($"꺼냄: {item1.Name}");

pool.Return( item1 );
Product item2 = pool.Get();
item2.Name = "복사본";
Console.WriteLine($"재사용: {item2.Name}");
Console.WriteLine($"꺼냄: {item1.Name}");    // 참조타입 출력 확인 

public class Product
{
    public string Name { get; set; } = "새 상품";
}
public class Pool<T> where T : class, new()
{
    private Queue<T> _pool = new Queue<T>();

    public T Get()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }
        return new T();
    }

    public void Return(T item)
    {
        _pool.Enqueue(item);
    }
}


// 12. 타입 매개변수를 열어둔 상속  -  데이터 저장, 조회
LoggingContainer<string> container = new LoggingContainer<string>();
container.Store("테스트 데이터");
string data = container.Get();
public class Container<T>
{
    protected T _item;

    public virtual void Store(T item)
    {
        _item = item;
    }

    public virtual T Get()
    {
        return _item; 
    }
}

public class LoggingContainer<T> : Container<T>
{
    public override void Store(T item)
    {
        Console.WriteLine($"[로그] 저장: {item}");
        base.Store(item);
    }

    public override T Get()
    {
        T item = base.Get();
        Console.WriteLine($"[로그] 조회: {item}");
        return item;
    }
}


//13. 닫힌 타입으로 상속 - 스택형태를 구현
IntStack stack = new IntStack();
stack.Push(10);
stack.Push(20);
stack.Push(30);

Console.WriteLine($"합계: {stack.Sum()}");
Console.WriteLine($"Pop: {stack.Pop()}");
Console.WriteLine($"남은 개수: {stack.Count}");
public class Stack<T>
{
    protected List<T> _items = new List<T>();

    public virtual void Push(T item)
    {
        _items.Add(item); 
    }
    public virtual T Pop()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("스택이 비어있습니다.");
        }
        T item = _items[_items.Count - 1];
        _items.RemoveAt(_items.Count -1);
        return item;
    }
    public int Count => _items.Count;
}

public class IntStack : Stack<int>  // 리스트의 합계 구하는 메서드
{
    public int Sum()
    {
        int total = 0;
        foreach (var item in _items)
        {
            total += item;
        }
        return total;
    }
}

//14. 새로운 타입 매개변수 추가
KeyedRepository<string, int> repo = new KeyedRepository<string, int>(s => s.Length);
repo.Add("사과");
repo.Add("바나나");

string found = repo.FindByKey(2);
Console.WriteLine($"길이 2인 항목: {found}");

public class Repository<T>
{
    protected List<T> _itmes = new List<T>();

    public void Add(T item)
    {
        _itmes.Add(item); 
    }
}
public class KeyedRepository<T, TKey> : Repository<T>
{
    private Dictionary<TKey, T> _index = new Dictionary<TKey, T>();
    private Func<T,  TKey> _keySelector;

    public KeyedRepository(Func<T, TKey> keySelector) // 데이터에서 키값을 추출하는 규칙메서드를 생성자로 받음
    {
        _keySelector = keySelector; 
    }

    public new void Add(T item)
    {
        base.Add(item);
        TKey key = _keySelector(item);
        _index[key] = item;
    }
    public T FindByKey(TKey key)
    {
        if (_index.TryGetValue(key, out T item))
        {
            return item;
        }
        return default(T);
    }
}

//15. 정적 데이터와 제네릭 - 타입별 생성 통계를 추적하는 클래스
var a = new Tracker<int>();
var b = new Tracker<int>();
var c = new Tracker<int>();
var d = new Tracker<string>();
var e = new Tracker<bool>();
var f = new Tracker<bool>();

Tracker<int>.PrintStats();
Tracker<string>.PrintStats();
Tracker<bool>.PrintStats();

Console.WriteLine($"마지막 int 인스턴스 ID: {c.InstanceId}");

public class Tracker<T>
{
    public static int TotalCreated { get; private set; } = 0;
    public int InstanceId { get; private set; }

    public Tracker()
    {
        TotalCreated++;
        InstanceId = TotalCreated;
    }

    public static void PrintStats()
    {
        Console.WriteLine($"Tracker<{typeof(T).Name}> -> 총 {TotalCreated}개 생성됨");
    }
}

//16. default 키워드
var inventory = new SimpleInventory<string>(3);
inventory.SetSlot(0, "검");
inventory.SetSlot(1, "방패");

inventory.PrintAll();

Console.WriteLine();
inventory.ClearSlot(0);
inventory.PrintAll();
public class SimpleInventory<T>
{
    private T[] _slots;

    public SimpleInventory(int size)
    {
        _slots = new T[size]; 
    }

    public void SetSlot(int index, T item)
    {
        _slots[index] = item;
    }

    public void ClearSlot(int index)
    {
        _slots[index] = default;
        Console.WriteLine($"슬릇 {index} 초기화됨");
    }

    public void PrintAll()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            string display = _slots[i] == null ? "(비어 있음)" : _slots[i].ToString();
            Console.WriteLine($"  슬릇 {i}: {display}");
        }
    }
}
*/
//17.  제네릭 스택 구현
MyStack<string> stack = new MyStack<string>();

stack.Push("첫 번째");
stack.Push("두 번째");
stack.Push("세 번째");
stack.Push("네 번째");
stack.Push("다섯 번째");

Console.WriteLine($"개수: {stack.Count}");
Console.WriteLine($"Peek: {stack.Peek()}");

while (!stack.IsEmpty)
{
    Console.WriteLine($"Pop: {stack.Pop()}");
}
public class MyStack<T>
{
    private T[] _items;
    private int _count;  // 현재 들어있는 개수 (동시에 다음에 들어갈 인덱스 번호)
    private const int DefaultCapacity = 4;

    public MyStack()
    {
        _items = new T[DefaultCapacity];
        _count = 0;
    }

    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public void Push(T item)
    {
        if (_count == _items.Length)
        {
            Resize(_items.Length * 2);
        }

        _items[_count++] = item;
    }

    public T Pop()
    {
        if (IsEmpty) throw  new InvalidOperationException("스택이 비어있습니다.");

        _count--;
        T item = _items[_count];
        _items[_count] = default(T); //추출한 자리 비워주기
        return item;
    }

    public T Peek()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("스택이 비어있습니다.");
        }
        return _items[_count -1];
    }

    private void Resize(int newSize)
    {
        T[] newArray = new T[newSize];
        for (int i = 0; i < _count; i++)
        {
            newArray[i] = _items[i];
        }
        _items = newArray;
        Console.WriteLine($"[내부] 용량 확장: {newSize}");
    }

}








