import tensorflow as tf
from tensorflow.python.platform import gfile
tf.compat.v1.disable_eager_execution()
with tf.compat.v1.Session() as sess:
    model_filename ='frozen_baseball.pb.bytes'
    #model_filename ='frozen2.pb.bytes'
#    model_filename ='frozen.pb.bytes'
    with gfile.FastGFile(model_filename, 'rb') as f:
        graph_def = tf.compat.v1.GraphDef()
        graph_def.ParseFromString(f.read())
        g_in = tf.import_graph_def(graph_def)
LOGDIR='./whatever'
print(sess.graph.as_graph_def())
train_writer = tf.compat.v1.summary.FileWriter(LOGDIR)
train_writer.add_graph(sess.graph)
train_writer.flush()
train_writer.close()
