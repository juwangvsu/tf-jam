Video:
tensorflowjs frozen graph keras tf-jam.mp4

------2/22/2020 git automatically change unix end of line fix-------------------
It change to windows eol crlf, unix eof is 0a, window is 0d0a, 
Verify this using hexedit, git by default do the conversion, this
Cause problem in bash. to disable that:
git config --global core.autocrlf false

----------2/21/2020 modify build.sh for docker image tf2cpu-keras-node-tfjs at win10 ---
bash has some issue when run the docker image at win10,
workaroud:
	tsjs_to_keras.sh replaced by "tensorflowjs_converter  ..." line

----------2/21/2020 docker win10 http server port map valumne share---------------------
Asus1 , on win10
Docker container and host not on same subnet, so ping not work
If run a webserver in container, must use -p to map port 
To share volumn with -v, must select c: for file share in Docker Desktop
The container Dockfile must expose port (such as 8080) to allow mapping.

Test step 
	docker run -it -p "8080:8080" -p “3000:6006” -v "c:\users\ju wang\documents\tf-jam:/app" jwang3vsu/tensorflow:tf2-keras-nodejs-tfjs bash
for http server:
	cd /app/tsjs/http; source ~/.profile; 
	node app.js
	At host browser, localhost:8080, should see “hello world”
for tensorboard:
	cd /app/tsjs
	./build.sh
	tensorboard --bind_all --logdir log3
	At host browser, localhost:3000

---------------2/20/2020 expose port -p tensorboard --bind_all ---------------------
when access a container port, some tricky parts:
	- Dockfile, EXPOSE 8080, required for the port to be accessed, 
		that port will be directly accessable on the container ipaddress:8080
	- -p option only allow a convenient way to access the exposed port via
		localhost, without having to use the container ip address
	- tensorboard  by default only bind to localhost, that will not be accessible from
		outside the container, use --bind_all to bind the port to all ip address
		of the container.
	to test, cd http, node app.js, then in host browser 172.17.0.2:8080
			here 0.2 address is the container ip address

-------------- 2/19/2020 docker image to generate frozen2.pb.bytes--------------------
cd tsjs;
docker run --gpus all -it -p "3000:6006" -v "${PWD}:/app" jwang3vsu/tensorflow:tf2-keras-nodejs-tfjs bash
	inside docker image:
		cd /app
		source ~/.profile
		./build.sh
	sed $'s/\r$//' ./build.sh > build.unix.sh		
		if run @ win10 and get /usr/bin/env: ‘bash\r’: No such file or directory
		this is build.sh when git pull in win10 somehow have different new line

-------------- 2/19/2020 pythonven setup to generate frozen2.pb.bytes--------------------
Msi, sda18, use pythonven
    source ~/python36-envs/sample-env/bin/activate
    (sample-env) student@student-Q302LA:~/unityproj/tf-jam/tsjs$ 
    source ~/bin/usecleanpythonpath.sh
    npm install (if node_modules not exist
    ./build.sh
        This is a mixed setup, (1) tensorflowjs_converter is from tf 2.0, (2)
        Import_pb_to_tensorboard.py is from tf1.14

node index.js
	Generate model_as_tsjs/model.json, weights.bin
bash tsjs_to_keras.sh
	Generate model_as_keras/model.h5
python3 keras_to_pb.py
	Generate frozen2.pb.bytes, model/tf_model.pb, model_as_pb/model.pb
cp frozen2.pb.bytes ../Assets/Resources/frozen2.pb.bytes
python3 /usr/local/lib/python3.5/dist-packages/tensorflow/python/tools/import_pb_to_tensorboard.py  --model_dir ./frozen2.pb.bytes --log_dir log3
	visualize the frozen file to a tensorboard folder

---------------------------2/19/2020 import_pb_to_tensorboard---------------------------
/usr/local/lib/python3.5/dist-packages/tensorflow/python/tools/import_pb_to_tensorboard.py

keras_ex3$ python keras_tensorboard.py 
	Train and generate keras_ex2_model.h5
keras_ex3$ python keras_to_pb.py
	Convert keras_ex2_model.h5 to 
Frozen2.pb.bytes
	with tf.gfile.GFile("frozen2.pb.bytes", "wb") as f:
        		f.write(frozen_graph.SerializeToString()
	Acceptable by (3)
model/tf_model.pb
	tf.train.write_graph((frozen_graph, "model", "tf_model.pb", as_text=False)
	Acceptable by (3)
model_as_pb/model.pb
	tf.train.write_graph(sess.graph_def, "./model_as_pb", "model.pb", as_text=False)
.Acceptable by (3), but if as_text=true then not accepatabe in (3)
python3 /usr/local/lib/python3.5/dist-packages/tensorflow/python/tools/import_pb_to_tensorboard.py  --model_dir model/tf_model.pb --log_dir log3
		Extract info from pb file for tensorboard to visualize



---------------------keras model examine in tensorboard -------------------------------------
/media/student/voc2012/unityproj/keras_ex3	
Keras_tensorboard.py train keras model a linear function, it save tensorboard information at logs. 

-------2/18/2020 saved_model tensorflow and visulize in tensorboard  ---------------------
(1): train and generate saved_model.pb
(sample-env) student@student-Q302LA:/media/student/voc2012/unityproj/mnist_saved_model$ python mnist_saved_model.py logs

(2) convert saved_model.pb to a tensorboard folder
(sample-env) student@student-Q302LA:/media/student/voc2012/unityproj/keras_ex3$ python import_pb_to_tensorboard.py --model_dir ../mnist_saved_model/logs/1/ --log_dir log2

(3) view tensorboard.
(sample-env) student@student-Q302LA:/media/student/voc2012/unityproj/keras_ex3$ tensorboard --logdir=log2


