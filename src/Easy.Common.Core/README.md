# Easy.Common.Core

常用方法封装库，提供了一些常用的扩展方法和工具类，帮助开发者提高开发效率。

## 安装

```sh
dotnet add package Easy.Common.Core --version 2025.04.28.2
```
# 1. 通用结果类
### 1.1 ApiResult
```cs 
//通用结果类ApiResult<T>,默认ApiResult<string>
ApiResult.Ok("成功");
ApiResult.Ok("成功", new { Id = 1, Name = "张三" });
ApiResult.Fail("失败");
ApiResult.Fail("失败",HttpStatusCode.InternalServerError);
ApiResult.Fail("失败", new { Id = 1, Name = "张三" });
ApiResult.Fail("失败", new { Id = 1, Name = "张三" }, HttpStatusCode.InternalServerError);
```
### 1.2 ApiResultPlus
ApiResultPlus<TSuccess, TError> 是一个扩展的通用结果类，用于处理可能返回两种不同类型结果的场景：成功结果和错误结果。它的作用是提供一种更灵活的方式来封装操作结果，尤其是在需要区分成功和失败的情况下。

作用
区分成功和失败的结果类型：

TSuccess 表示成功时返回的数据类型。
TError 表示失败时返回的数据类型。
简化结果处理逻辑：

通过 Match 方法，可以分别对成功和失败的结果进行处理，而无需手动检查状态。
提高代码可读性：

使用 ApiResultPlus 可以让代码更清晰地表达操作的意图，避免混淆成功和失败的处理逻辑。
示例解释
以下代码展示了 ApiResultPlus 的使用：
```csharp
public ApiResult GetResult(int type)
{
    // 创建 ApiResultPlus 实例，表示成功返回类型为 string，失败返回类型为 ErrorInfo
    var result = new ApiResultPlus<string, ErrorInfo>();

    // 模拟业务逻辑
    if (type == 1)
        result = "成功";  // 设置成功数据
    else
        result = ErrorInfo.Error("发生错误");  // 设置失败信息

    // 使用 Match 方法处理成功和失败的结果
    var res = result.Match(
        success =>
        {
            // 处理成功情况
            return ApiResult.Ok(success);  // 返回成功的 ApiResult
        },
        error =>
        {
            // 处理失败情况
            return ApiResult.Fail(error.Msg, error.Data);  // 返回失败的 ApiResult
        }
    );
    return res;  // 返回处理后的结果
}
```
### 1.3 其他通用类
```
//分页
IPageList<T>
```

# 2. 常用扩展方法
``` csharp
// 空值判断
bool isStringNull = "".IsNull(); // 返回 true
List<int> list = new List<int>();
bool isListNull = list.IsNull(); // 返回 true
object obj = null;
bool isObjectNull = obj.IsNull(); // 返回 true

// 判断字符串是否为手机号码
bool isMobile = "12345678910".IsMobile(); // 返回 true
// 检测是否符合email格式
bool isEmailValid = "test@example.com".IsValidEmail(); // 返回 true
// 检测是否是正确的Url
bool isUrlValid = "https://www.example.com".IsUrl(); // 返回 true
// string 转 int 数组
int[] intArray = "1,2,3,4".StringToIntArray(); // 返回 {1, 2, 3, 4}
// String 转 string 数组
string[] stringArray = "apple,banana,orange".StringToStringArray(); // 返回 {"apple", "banana", "orange"}
// String 数组转 Int 数组
int[] intArrayFromStrArray = new string[] { "1", "2", "3" }.StringArrAyToIntArray(); // 返回 {1, 2, 3}
// string 转 Guid 数组
Guid[] guidArray = "f47ac10b-58cc-4372-a567-0e02b2c3d479,f47ac10b-58cc-4372-a567-0e02b2c3d480".StringToGuidArray(); // 返回 [{Guid1}, {Guid2}]
// 获取32位md5加密
string md5_32 = "password".Md5For32(); // 返回 32位的 MD5 字符串
// 获取16位md5加密
string md5_16 = "password".Md5For16(); // 返回 16位的 MD5 字符串
// 清除HTML中指定样式
string cleanHtml = "<div style='color:red;'>Text</div>".ClearHtml(new string[] { "style" }); // 返回 "<div>Text</div>"
// list 随机排序方法
List<int> sortedList = new List<int> { 1, 2, 3, 4, 5 }.RandomSortList(); // 返回随机排序的 List
// 截前后字符(串)
string interceptedText = "hello world".GetCaptureInterceptedText("world", all: true); // 返回 "hello"
// 密码加密方法
string encryptedPassword = "password".EnPassword(DateTime.Now); // 返回加密后的密码
// URL编码
string encodedUrl = "https://example.com?name=value".UrlEncode(); // 返回 URL 编码的字符串
// 获取10位时间戳
long timestamp10 = FoundationExtensions.GetTimeStampByTotalSeconds(); // 返回当前时间的10位时间戳
// 获取13位时间戳
long timestamp13 = FoundationExtensions.GetTimeStampByTotalMilliseconds(); // 返回当前时间的13位时间戳
// HmacSHA256加密
string hmacSHA256 = FoundationExtensions.HmacSHA256(1609459200, "secret"); // 返回 HMACSHA256 加密后的字符串
```
# 3. 时间扩展类
``` csharp
// 获取当前的毫秒时间戳
string msectime = DateTimeExtensions.Msectime(); // 返回当前的毫秒时间戳
// 获取剩余多久时间的文字描述
string remainingTimeDescription = DateTimeExtensions.GetRemainingTime(DateTime.Now.AddHours(1)); // 返回类似 "1小时0分0秒"
// 获取剩余多久时间的具体时间类型
int day, hours, minute, seconds;
DateTimeExtensions.GetBackTime(DateTime.Now.AddHours(1), out day, out hours, out minute, out seconds); // 返回具体的天、小时、分钟、秒
// 计算时间戳剩余多久时间
string timeAgo = DateTimeExtensions.TimeAgo(DateTime.Now.AddHours(-2)); // 返回类似 "2小时前"
// 获取当前是星期几
string currentWeek = DateTimeExtensions.GetWeek(); // 返回 "周一", "周二", "周三" 等
```
# 4. 字典工具类
``` csharp
// 创建一个对象用于测试
var obj = new { Name = "Alice", Age = 28, Occupation = "Engineer" };

// 使用 DictionaryExtensions 类转换对象为字典
IDictionary<string, object> dictionary = DictionaryExtensions.ToDictonary(obj);

// 输出字典内容
foreach (var kvp in dictionary)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}
// 输出:
// Name: Alice
// Age: 28
// Occupation: Engineer
```
# 5. 加密解密帮助类
``` csharp
// 原始文本
string plainText = "Hello, this is a secret message!";

// 使用 EncryptionExtensions 加密并缩短文本
string encryptedText = EncryptionExtensions.EncryptAndShorten(plainText);
Console.WriteLine("Encrypted and Shortened Text: " + encryptedText);

// 使用 EncryptionExtensions 解密并还原文本
string decryptedText = EncryptionExtensions.DecryptAndUnshorten(encryptedText);
Console.WriteLine("Decrypted and Unshortened Text: " + decryptedText);

// 输出结果:
// Encrypted and Shortened Text: <Base64Url Encoded Encrypted Data>
// Decrypted and Unshortened Text: Hello, this is a secret message!

```
# 6. 文件帮助类
``` csharp
// 获取文件夹名称
string filePath = @"C:\Users\Example\Documents\sample.txt";
string directoryName = FileExtensions.GetDirectoryName(filePath);
Console.WriteLine("文件夹名称: " + directoryName);
// 输出: 文件夹名称: C:\Users\Example\Documents

// 获取文件夹下的所有文件
string[] files = FileExtensions.GetFiles(@"C:\Users\Example\Documents", "*.txt", SearchOption.AllDirectories);
Console.WriteLine("文件列表: " + string.Join(", ", files));

// 获取文件内容
string fileContent = FileExtensions.GetFileConnent(filePath);
Console.WriteLine("文件内容: " + fileContent);

// 获取文件信息
FileInfo? fileInfo = FileExtensions.GetFileInfo(filePath);
if (fileInfo != null)
{
    Console.WriteLine("文件大小: " + fileInfo.Length + " 字节");
}

// 文件备份
bool backupSuccess = FileExtensions.FileBackup(filePath);
Console.WriteLine("文件备份成功: " + backupSuccess);

// 获取文件内容（异步）
Task.Run(async () =>
{
    string asyncFileContent = await FileExtensions.GetFileConnentAsync(filePath);
    Console.WriteLine("异步获取的文件内容: " + asyncFileContent);
});

// 创建文件
string newFilePath = @"C:\Users\Example\Documents\newfile.txt";
string content = "你好，这是一个新文件！";
FileExtensions.CreateFile(newFilePath, content);
Console.WriteLine("新文件已创建，路径为: " + newFilePath);

// 创建文件夹
string newFolderPath = @"C:\Users\Example\Documents\NewFolder";
FileExtensions.CreateDirectory(newFolderPath);
Console.WriteLine("文件夹已创建，路径为: " + newFolderPath);
```
# 7. 数据转换
``` csharp
 // 1. 将对象转换为基本数据类型
object obj1 = "123";
int intValue = obj1.ToInt();  // intValue = 123
object obj2 = "12345678901234";
long longValue = obj2.ToLong();  // longValue = 12345678901234
object obj3 = "3.14";
double doubleValue = obj3.ToDouble();  // doubleValue = 3.14
object obj4 = "Hello, World!";
string stringValue = obj4.ToStringValue();  // stringValue = "Hello, World!"
object obj5 = "10.5";
decimal decimalValue = obj5.ToDecimal();  // decimalValue = 10.5
object obj6 = "2025-04-28";
DateTime dateValue = obj6.ToDate();  // dateValue = 2025-04-28
object obj7 = "true";
bool boolValue = obj7.ToBool();  // boolValue = true
Console.WriteLine($"intValue: {intValue}, longValue: {longValue}, doubleValue: {doubleValue}, stringValue: {stringValue}, decimalValue: {decimalValue}, dateValue: {dateValue},boolValue: {boolValue}");
// 2. 将字典转换为实体对象
var dict = new Dictionary<string, object>
{
    { "Id", 1 },
    { "Name", "John Doe" },
    { "Age", 30 }
};
var person = dict.ToEntity<Person>();  // Person: Id = 1, Name = "John Doe", Age = 30
Console.WriteLine($"Person: Id = {person.Id}, Name = {person.Name}, Age = {person.Age}");
// 3. 将字典列表转换为实体对象列表
var dictList = new List<Dictionary<string, object>>()
{
    new Dictionary<string, object>
    {
        { "Id", 1 },
        { "Name", "John Doe" },
        { "Age", 30 }
    },
    new Dictionary<string, object>
    {
        { "Id", 2 },
        { "Name", "Jane Smith" },
        { "Age", 28 }
    }
};
var peopleList = dictList.ToList<Person>();  // peopleList 包含两个 Person 对象
foreach (var p in peopleList)
{
    Console.WriteLine($"Person: Id = {p.Id}, Name = {p.Name}, Age = {p.Age}");
}
// 4. 将字符串转换为指定类型
string strValue = "123.45";
double doubleResult = strValue.ToType(typeof(double));  // doubleResult = 123.45
string dateString = "2025-04-28";
DateTime dateResult = (DateTime)dateString.ToType(typeof(DateTime));  // dateResult = 2025-04-28
Console.WriteLine($"doubleResult: {doubleResult}, dateResult: {dateResult}");
// 5. 将字符串转换为数值类型
string str = "100";
int? intValueNullable = str.ToValue<int>();  // intValueNullable = 100
string boolStr = "true";
bool? boolValueNullable = boolStr.ToValue<bool>();  // boolValueNullable = true
string decimalStr = "50.75";
decimal? decimalValueNullable = decimalStr.ToValue<decimal>();  // decimalValueNullable = 50.75
Console.WriteLine($"intValueNullable: {intValueNullable}, boolValueNullable: {boolValueNullable}, decimalValueNullable: {decimalValueNullable}");
// 6. 处理 null 值的转换
object nullValue = null;
int defaultIntValue = nullValue.ToInt();  // defaultIntValue = 0
object dbNullValue = DBNull.Value;
DateTime defaultDateValue = dbNullValue.ToDate();  // defaultDateValue = DateTime.MinValue
Console.WriteLine($"defaultIntValue: {defaultIntValue}, defaultDateValue: {defaultDateValue}");
```



详细文档请访问 [项目主页](https://gitee.com/wangbenchi66/nuget)。