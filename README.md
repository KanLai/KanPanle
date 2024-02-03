
💡 Tips：

1. wget https://github.com/KanLai/KanPanle/archive/refs/heads/master.zip
2. unzip master
3. cd master
4. docker build -t 镜像名称 -f WebApplication1/Dockerfile-*** .
5. 生成镜像后，run即可。指定端口
6. docker run -d --name=**** --restart=always -p 8080:8080 镜像名称
7. 如需保存数据到本地 -v 路径 /web 即可。

