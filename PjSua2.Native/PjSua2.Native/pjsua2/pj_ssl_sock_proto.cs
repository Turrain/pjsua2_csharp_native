//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.2.0
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace PjSua2.Native.pjsua2 {

public enum pj_ssl_sock_proto {
  PJ_SSL_SOCK_PROTO_DEFAULT = 0,
  PJ_SSL_SOCK_PROTO_SSL2 = 1 << 0,
  PJ_SSL_SOCK_PROTO_SSL3 = 1 << 1,
  PJ_SSL_SOCK_PROTO_TLS1 = 1 << 2,
  PJ_SSL_SOCK_PROTO_TLS1_1 = 1 << 3,
  PJ_SSL_SOCK_PROTO_TLS1_2 = 1 << 4,
  PJ_SSL_SOCK_PROTO_TLS1_3 = 1 << 5,
  PJ_SSL_SOCK_PROTO_SSL23 = (1 << 16) -1,
  PJ_SSL_SOCK_PROTO_ALL = (1 << 16) -1,
  PJ_SSL_SOCK_PROTO_DTLS1 = 1 << 16
}

}
