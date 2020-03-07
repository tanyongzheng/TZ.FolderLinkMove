# TZ.FolderMove

TZ文件夹链接转移程序

## 使用场景
1. C盘安装的程序转移（如微信、PostMan等安装在C盘的AppData\Local目录下）到其他磁盘不破坏之前的程序及快捷方式

2. 部分程序的缓存目录为C盘（如Chrome等浏览器缓存、Visual Studio 2017及以上在线安装的缓存文件），转移到其他盘

## 参数说明
默认转为的链接为目录符号链接(directory symbolic link ,mklink参数为/d）,该方式可以跨本地磁盘及网络磁盘（共享目录）

也可以选目录连接(Directory Junction,mklink参数为/j）,该方式只能跨本地磁盘

查看链接类型可以使用NTFSLinksView这个软件，Symbolic Link就是符号链接，Junction是目录连接

## CMD命令处理
1. 将文件夹剪切到目标位置（先复制后删除比较保险）

2. 进入CMD命令行，输入命令 mklink /d "要转移的文件夹" "目标文件夹"　<br />
譬如 mklink /d "C:\Users\Administrator\AppData\Local\Postman" "E:\Program Files (x86)\Postman"　

## 注意
如果要把文件夹移回原来的位置，则需要先把删除该链接（符号链接或目录连接）

[mklink官方文档](https://docs.microsoft.com/zh-cn/windows-server/administration/windows-commands/mklink)

[mklink英文文档](ttps://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-R2-and-2012/cc753194(v=ws.11)?redirectedfrom=MSDN)

[mklink参数详解](https://www.jianshu.com/p/b1614a073087)

[NTFSLinksView下载地址](http://www.nirsoft.net/utils/ntfs_links_view.html)