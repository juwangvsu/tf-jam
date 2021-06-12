Video:
tensorflowjs frozen graph keras tf-jam.mp4
video: “unity ml agent test 1.mp4”
	“unity tf-jam mlagent test 2.mp4”

mlagents repo: https://github.com/Unity-Technologies/ml-agents
	it has python package and unity package, each with different version #
	python package is installed via pip 
	unity package is installed via "package manager" inside unity editor software

------------------------6/11/21 mlagents works after upgrade mlagents to 1.0.0 -------------
msi: unity 2018.4
see readme in rollerball proj

RollerAgent.cs runs now, it still not doing anything useful, just move around.
the rolleragent.cs is to replace ballspawnercontroler.cs, which use tensorflowsharp plugin.
tensorflowsharp.dll plugin provide impl for "using TensorFlow;" in c# code

---------------------------6/10/21 retest mlagents control drone (rollerball) ------------
msi laptop
i9lab-win
	conda env:
		mlagents: (python package: mlagents==0.25.1)   (unity package plugin 1.9.1), (github branch release 16)
		mlagents-015: (python package: mlagents=0.16.0 (unity package plugin 1.0.0),(github branch release 0.15.1
)
	git clones: 
		h:\mlagents    release 16
		h:\mlagents-0.15.1    release 0.15.1


issue: the training cmd line not work with unity game. possible due to mismatch btw python interface and unity mlagents library.
 previous installation: the cmd line show:
	  ml-agents: 0.14.0
	  ml-agents-envs: 0.14.0,
	  Communicator API: API-14
	  TensorFlow: 2.0.1
 new (mlagents) Version information:
	  ml-agents: 0.25.1,
	  ml-agents-envs: 0.25.1,
	  Communicator API: 1.5.0,
	  PyTorch: 1.7.1+cu110
status:
	rollerball finally worked again in i9labwin: conda env: mlagents-015, unity package manager mlagents version preview 1.0. change of unity package from 0.16 to 1.0.0: using MLAgents replaced by using Unity.MLAgents.

-----6/10/21 re-create win10 msi environment ------------------
3/4/20 note not working. python venv fail, possibly due to conda. now try to setup conda env
   follow installation note: https://github.com/Unity-Technologies/ml-agents/blob/release_16_docs/docs/Installation.md
   mlagents release 16, compatible with unity3d 2018.4, the previous install might be release 14.
   
  M:
    git clone --branch release_16 https://github.com/Unity-Technologies/ml-agents.git

  python env setup with conda: mlagents release 16 
	conda create -n mlagents python=3.7
	conda activate mlagents
	conda install -c conda-forge implicit
	pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html
	python -m pip install mlagents==0.25.1
		this will also install executable shell script mlagents-learn.exe in path
  python env activate:
	conda activate mlagents

  python env setup with conda: mlagents release 0.15.1 or 0.16 
	in conda env mlagents-015 
	pip install mlagents==0.16.0
		this install tensorflow 2.5

  issues:
	F:\unityproj\tf-jam>mlagents-learn trainer_config.yaml --run-id=ball3 --train --time-scale=100
	    error about the yaml file
	mlagents-learn --run-id=ball3 --train --time-scale=100
            run but time out , not able to comm with tf-jam game, check the tf-jam code to make sure the agent is enable. 

  RollerAgent as of 3/4/2020:
	it only move the player around, but not shooting the ball?

---------3/4/2020 tf-jam RollerAgent training status -----------------------------------
	training cmd works, traing logic still under studying...
	tf-jam training no improvement, start from simple training goal similar to simplemover,
	to understand reward scheme.

----------3/4, 2/25/2020 unity3d win10 mlagent new proj training-------------------------
Msi win10, https://github.com/Unity-Technologies/ml-agents/blob/latest_release/docs/Learning-Environment-Create-New.md
F:\unitproj\mlagents, F:\unitproj\rollerball 
	Pip install mlagents (this installed all dep packages, such tf2, etc)
Mlagent 3dball ok, 
train ok: train cmd mlagents-learn is a python script, using mlagents package, must be run from venv where mlagents and tf are installed. Unity3d software is independ to the mlagents as a python package. Unity3d editor gui runs as usual, it do not need python.
Train steps:
Python venv @ c:\Users\Ju Wang\Documents\python-env>sample-env\Scripts\activate
(sample-env) F:\unityproj\tf-jam>mlagents-learn trainer_config.yaml --run-id=ball2 --train --time-scale=100
Click play at unity3d, in terminal (2) shows training start and interact with unity3d, click stop at unity3d to terminate training.
Results: "models\ball2\My Behavior.nn", “F:\unityproj\tf-jam\summaries\ball2_My Behavior” for tensorboard
---Steps to create a new learning proj:
	---See url above for detail steps.
	--- summary: create a new unity proj, add ML-agent package through package manager, create physical agent body (e.g., a sphare), add c# script subclass Agent, add two scripts provided by ML (click agent body, add components->ML Agents-> decision requiest script. Do the same to add “Behavior Parameter” script. So this agent has three relevent scripts directely added. The behavior parameter script describe the input/output of the nn. The main agent subclass script impl observation, action execution, and reset. 

-------------3/3/2020 test AddForce mode: impulse, velocity, magic formula -------------
ball shoot is done in BrickController.cs, Start():
	 GetComponent<Rigidbody>().AddForce(Force, ForceMode.VelocityChange);
magic formular: vy = (deltay + 5 * Mathf.Pow(t, 2)) / t; 
		vx=5, t = dist/vx, assume same z goal and ball
see RollerAgent.cs

------------3/2/2020 code logic -----------------------------------------
BrickController.cs
	attached to Ball gameobject
RollerAgent.cs
	Huristic called if nn model is empty, which is set in unity editor, behavior parameter component
	attached to Player gameobject, Player instanciate a ball gameobject, set its force attribute. The ball
	Start() start a timed invocation of Doshoot()
	AgentAction() save action, called upon very frequently, check reward we choose to not spawn ball instance here.
	DoShoot() wakeup every so often, spawn a new ball, set its force. new ball life start when Doshoot() yield
BallSpawnerController_2.cs
	attached to Player gameobject, not enable to give RollerAgent full control.

------2/23/2020 tf-jam debugging force velocity ---------------------------
	force = (-0.5, force,0) if > 10 ft, (-0.xx, force, 0) 0.xx <0.5
	magical formular works for mostly > 10 ft.
	the x direction force decrease if ball close to post
	print velocity in BallController.cs, which is is script for the prefab Ball
	object.
	because the way Force is assigned, the succ shot is more consistent for dist > 10 ft
TBD:	close range shot are bad. seek for remification.
		
	despawn to destroy an object, yield to wait, velocity vi RigidBody rb.velocity

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
	if win10, delete node_modules, and npm install if needed.
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


