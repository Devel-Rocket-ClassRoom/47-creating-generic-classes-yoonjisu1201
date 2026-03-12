using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
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
*/

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

