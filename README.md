[English Readme is here](https://github.com/EH500-Kintarou/ListViewExtensions/blob/master/README-en.md)

# ListView Extensions
WPFのListViewは効果的にコレクションの内容を表示することができますが、ヘッダーをクリックしてソートしたり、右クリックやダブルクリック、選択したアイテムの取得などの機能を実装するととたんに難しくなります。  
ListView Extensionsは、そのようなListViewのポテンシャルを簡単に引き出すために、View / ViewModel / Modelの各面からサポートをするライブラリです。

![](https://img.shields.io/badge/Nuget-1.1.0-blue?logo=nuget&style=plastic)
![](https://img.shields.io/badge/.NET_Framework-4.5.2-orange?logo=.net&style=plastic)
![](https://img.shields.io/badge/.NET_Core-3.1-orange?logo=.net&style=plastic)
![](https://img.shields.io/badge/.NET-6-orange?logo=.net&style=plastic)

![Screenshot of ListView Extensions](https://raw.githubusercontent.com/EH500-Kintarou/ListViewExtensions/master/Images/SampleScreenshot.png)

## 主な機能
- ソート
- 項目の複数選択のバインディング
- 項目のダブルクリックのViewModel通知
- スレッドセーフなコレクション操作

![Class Relationship Overview](https://raw.githubusercontent.com/EH500-Kintarou/ListViewExtensions/master/Images/ClassRelationshipOverview_ja.png)

## 動作環境
- .NET Core 3.1以上 / .NET Framework 4.5.2以上

## 使用方法
### 1. Nugetからインストール
![](https://img.shields.io/badge/Nuget-1.1.0-blue?logo=nuget&style=plastic) https://www.nuget.org/packages/ListViewExtensions

### 2. XAML名前空間を設定
XAMLで名前空間 "http://schemas.eh500-kintarou.com/ListViewExtensions" を設定します。

```xaml
<Window x:Class="ListViewExtensionsTest.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
        xmlns:ei="http://schemas.microsoft.com/expression/2010/interactions"
		xmlns:lv="http://schemas.eh500-kintarou.com/ListViewExtensions"
        Title="MainWindow" Height="480" Width="640">
```

### 3. Viewを作成
View内でListViewを作成します。その際、GridViewColumnで列を設定する代わりに、SortableGridViewColumnを使用することで、その列でソートすることができるようになります。

```xaml
<ListView ItemsSource="{Binding People}" >
	<ListView.View>
        <GridView>
            <lv:SortableGridViewColumn Width="120" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Name}" Header="Name" />
            <lv:SortableGridViewColumn Width="150" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Pronunciation}" Header="Pronunciation" />
            <lv:SortableGridViewColumn Width="70"  SortableSource="{Binding People}" DisplayMemberBinding="{Binding Age}" Header="Age" />
            <lv:SortableGridViewColumn Width="120" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Birthday}" Header="Birthday" />
            <lv:SortableGridViewColumn Width="120" SortableSource="{Binding People}" DisplayMemberBinding="{Binding Height}" Header="Height" />
		</GridView>
	</ListView.View>
</ListView>
```

### 4. Modelを作成
Model内でSortableObservableCollectionクラスをインスタンス化します。これはListViewで表示される内容に対応するコレクションで、スレッドセーフに作ってあります。

```cs
public SortableObservableCollection<PersonModel> People { get; } = new SortableObservableCollection<PersonModel>();
```

### 5. ViewModelを作成
ViewModel内でListViewViewModelクラスをインスタンス化します。ModelのSortableObservableCollectionの変更通知をUIスレッドに転送してくれるほか、並び変え、選択などのコマンドも持っていて、ListViewの多くの機能を受け止めることができます。

```cs
People = new ListViewViewModel<PersonViewModel, PersonModel>(model.People, person => new PersonViewModel(person), DispatcherHelper.UIDispatcher);
```

### 6. サンプルコードをチェック
このリポジトリにはサンプルコードが含まれています。 [サンプルコード](https://github.com/EH500-Kintarou/ListViewExtensions/tree/master/Sample) を見ることでより理解が深まります。

## プロジェクトURL
![](https://img.shields.io/badge/Github-1.1.0-green?logo=github&style=plastic) https://github.com/EH500-Kintarou/ListViewExtensions  
![](https://img.shields.io/badge/Nuget-1.1.0-blue?logo=nuget&style=plastic) https://www.nuget.org/packages/ListViewExtensions  
![](https://img.shields.io/badge/Blogger-1.1.0-orange?logo=blogger&style=plastic) https://days-of-programming.blogspot.com/search/label/ListView%20Extensions

## バージョン履歴
### ver.1.1.0 (2024/08/31)
- ターゲットを.NET Framework 4.5.2 / .NET Core 3.1 / .NET 6に変更
- ListViewのヘッダーサポートを強化
  - SortableGridViewColumnHeader、SortableGridViewColumnを追加
  - SortedHeaderをObsolete指定にした
- ISortableObservableCollectionのSortメソッドの引数を変更、古いメソッドはObsolete指定にした
- コードのリファクタリング、Nullableの有効化、単体テストの追加など

### ver.1.0.1 (2018/03/24)
- Obsoleteに指定していた非同期アクセス非サポートのクラスを削除
- SyncedObservableCollectionが実装する非ジェネリックインターフェイス（IList, ICollectionなど）を明示的なインターフェイスの実装に変更

### ver.1.0.0 (2017/11/26)
- 正式リリース
