from tensorflow.python.tools import import_pb_to_tensorboard
import_pb_to_tensorboard.import_to_tensorboard('./frozen2.pb.bytes','log3')
#direct cmd line:
#python3 /usr/local/lib/python3.5/dist-packages/tensorflow/python/tools/import_pb_to_tensorboard.py  --model_dir ./frozen2.pb.bytes --log_dir log3
