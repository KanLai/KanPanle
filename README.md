
💡 Tips：

1. 下载文件
2. 解压文件
3. docker build -t 镜像名称 -f WebApplication1/Dockerfile-*** .
4. 生成镜像后，run即可。指定端口
5. docker run -d --name=**** --restart=always -p 8080:8080 镜像名称
6. 如需保存数据到本地 -v 路径 /web 即可。

