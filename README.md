
💡 Tips：

1. 下载文件 wget https://github.com/KanLai/KanPanle/archive/refs/heads/master.zip
2. unzip master.zip
3. cd KanPanle-master
5. docker build -t 镜像名称 -f WebApplication1/Dockerfile-*** .
6. 生成镜像后，run即可。指定端口
7. docker run -d --name=**** --restart=always -p 8080:8080 镜像名称

