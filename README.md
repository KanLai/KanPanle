
💡 Tips：

wget https://github.com/KanLai/KanPanle/archive/refs/heads/master.zip
unzip master
cd master
docker build -t 镜像名称 -f WebApplication1/Dockerfile-*** .
生成镜像后，run即可。指定端口
docker run -d --name=**** --restart=always -p 8080:8080 镜像名称
如需保存数据到本地 -v 路径 /web 即可。

