#!/usr/bin/env python3
# 2/15/2020: keras and tensorflow.keras version not same could cause problem in .h5 format
#	keras is 2.3.1, tensorflow.keras is 2.2.4 msi sda18
# convert a h5 to pb format and a frozen format, result is in frozen2.pb.bytes, model_as_pb , model_as_checkpoint

from tensorflow.keras.models import load_model
#	this is 2.2.4
#  	print(tensorflow.keras.__version__)
#from keras.models import load_model
#	this is 2.3.1
#  	print(keras.__version__)

import tensorflow as tf
if tf.__version__.find("1")==0:
   print("version 1 found ")
else:
   import tensorflow.compat.v1 as tf
   tf.disable_v2_behavior()
   print("version 2 found ")
print("version set to found " + tf.__version__)

import sys
#sess = tf.compat.v1.Session()
sess = tf.Session()

from tensorflow.keras import backend as K
tf.compat.v1.keras.backend.set_session(sess)
#K.set_session(sess)

model_id=2
#model_id=1 keras_ex3, keras_ex2_model.h5, generated from keras_tensorboard.py
#	 =2 tsjs, model_as_keras/model.h5, generated from index.js
#model = load_model('keras_ex2_model.h5')
model = load_model('model_as_keras/model.h5')

#model = load_model('baseball_model.h5')
print(model.outputs)
print(model.inputs)
model.summary()
def freeze_session(session, keep_var_names=None, output_names=None, clear_devices=True):
    """
    Freezes the state of a session into a pruned computation graph.

    Creates a new computation graph where variable nodes are replaced by
    constants taking their current value in the session. The new graph will be
    pruned so subgraphs that are not necessary to compute the requested
    outputs are removed.
    @param session The TensorFlow session to be frozen.
    @param keep_var_names A list of variable names that should not be frozen,
                          or None to freeze all the variables in the graph.
    @param output_names Names of the relevant graph outputs.
    @param clear_devices Remove the device directives from the graph for better portability.
    @return The frozen graph definition.
    """
    from tensorflow.python.framework.graph_util import convert_variables_to_constants
    graph = session.graph
    with graph.as_default():
        freeze_var_names = list(set(v.op.name for v in tf.global_variables()).difference(keep_var_names or []))
        output_names = output_names or []
        output_names += [v.op.name for v in tf.global_variables()]
        input_graph_def = graph.as_graph_def()
        if clear_devices:
            for node in input_graph_def.node:
                node.device = ""
        frozen_graph = convert_variables_to_constants(session, input_graph_def,
                                                      output_names, freeze_var_names)
        return frozen_graph
if model_id==1:
   inp = sess.graph.get_tensor_by_name('layer1_input:0')
else:
   inp = sess.graph.get_tensor_by_name('hidden_input:0')

print(inp)
frozen_graph = freeze_session(sess, output_names=[out.op.name for out in model.outputs])

# this is acceptable to import_pb_to_tensorboard.py
with tf.gfile.GFile("frozen2.pb.bytes", "wb") as f:
        f.write(frozen_graph.SerializeToString())

#this is the same as frozen2.pb.bytes
tf.train.write_graph(frozen_graph, "model", "tf_model.pb", as_text=False)
print("%d ops in the final graph." % len(frozen_graph.node))

#tf.saved_model.simple_save(sess, "model_as_checkpoint/Profile.ckpt")
tSaver = tf.train.Saver()
tSaver.save(sess, "model_as_checkpoint/Profile.ckpt")

print([n.name for n in tf.get_default_graph().as_graph_def().node])
print(model.summary())

#this only has the graph definition in text format, not acceptable to import_pb_to_tensorboard.py
tf.train.write_graph(sess.graph_def, "./model_as_pb", "model.pb", as_text=False)
