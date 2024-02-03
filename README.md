
💡 Tips：

1. 下载文件：wget https://github.com/KanLai/KanPanle/archive/refs/tags/1.0.0.zip
2. unzip 1.0.0.zip
3. cd KanPanle-1.0.0
4. docker build -t kanlai/panle -f WebApplication1/Dockerfile-amd64 . (根据自己机器选择)
5. docker run -d --name=kanlai-panle --restart=always -p 8080:8080 kanlai/panle
6. 清理打包产生的镜像和垃圾。docker system prune -a -f 注意会删除所有未使用的镜像和容器 网络等
7. 访问http://ip:8080
8. 默认账号密码：admin/123456
9. 外网请及时修改密码
10. 请勿用于非法用途，否则后果自负
11. https 443端口需要自行配置 nginx代理即可。
12. 也可以自己修改Rider 打开 或者VS 自己配置。自行编译


