# Object2Soql

Object2Soql is a library to convert strongly typed C# expressions to a [SOQL](https://developer.salesforce.com/docs/atlas.en-us.222.0.soql_sosl.meta/soql_sosl/sforce_api_calls_soql_sosl_intro.htm) query sintax.

## Installation
TBD

```bash
pip install foobar
```

## Usage
```c#
using Object2Soql;

// SELECT MyProperty From MyClass OFFSET 2;
var query = Soql.From<TestClass>().Select((x)=>x.MyProperty).Skip(2).Build();

// SELECT MyProperty WHERE MyEnum = 'Case B' AND MyInt >= 2 From MyClass;
var query = Soql.From<TestClass>().Select((x)=>x.MyProperty).Where((x)=>x.MyEnum == Enums.CaseB && x.MyInt > 2)Build();

// SELECT MyProperty, MyChild.ItsProperty From MyClass;
var query = Soql.From<TestClass>().Select((x)=> new{ x.MyProperty, x.Child.Property }).Build();
```
### Available Methods
#### Select
#### Where
#### OrderBy
#### ThenBy
#### Take
#### Skip
#### GroupBy
#### Include
When no projection is done using `Select` the library will automaticall include all non-complex properties in the projection.
`Include` serves to specify a single complext property and include all its non-complext properties in the projection.
This method my be called several times.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)