# Build NNG native shared library for: linux-x64, linux-arm, linux-arm64
#
# USAGE:
# docker run --rm --privileged multiarch/qemu-user-static:register
# docker build -t jeikabu/build-nng dockerfiles/build_nng
# docker run -i -t --rm -v "$PWD/nng.NET/runtimes:/runtimes" -v "$PWD/nng.NET/licenses:/licenses" jeikabu/build-nng

ARG SRC=/usr/src

# Git NNG source
FROM ubuntu:focal AS nng

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get install -y \
    ca-certificates \
    curl \
    git

ARG SRC
ARG NNG_VERSION=1.11
ARG NNG_BRANCH=v${NNG_VERSION}
ARG MBEDTLS_VERSION=3.6.4
ARG MBEDTLS_BRANCH=v${MBEDTLS_VERSION}
WORKDIR ${SRC}

RUN git clone https://github.com/Mbed-TLS/mbedtls.git && \
    cd mbedtls && \
    git checkout ${MBEDTLS_BRANCH} && \
    git submodule update --init

RUN git clone https://github.com/nanomsg/nng.git &&\
    cd nng &&\
    git checkout ${NNG_BRANCH}

################################################################################
# Build x64 Linux
FROM ubuntu:focal AS linux-x64

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get install -y \
        build-essential \
        cmake \
        perl \
        python3 \
        python3-pip \
    && pip3 install Jinja2

ARG SRC
ARG RUNTIME_NATIVE=${SRC}/runtimes/linux-x64/native

WORKDIR ${SRC}
COPY --from=nng ${SRC}/mbedtls ${SRC}/mbedtls
RUN mkdir -p mbedtls/build && cd mbedtls/build \
    && cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release \
        -DUSE_STATIC_MBEDTLS_LIBRARY=OFF \
        -DUSE_SHARED_MBEDTLS_LIBRARY=ON \
        -DENABLE_TESTING=OFF -DENABLE_PROGRAMS=OFF \
        -DCMAKE_INSTALL_PREFIX=${SRC}/install-prefix .. \
    && mkdir -p install \
    && make -j2 \
    && make install

WORKDIR ${SRC}
COPY --from=nng ${SRC}/nng ${SRC}/nng
RUN mkdir -p nng/build && cd nng/build \
    && cmake -G "Unix Makefiles" -DBUILD_SHARED_LIBS=ON \
        -DCMAKE_BUILD_TYPE=Release \
        -DNNG_ENABLE_TLS=ON \
        -DCMAKE_PREFIX_PATH=${SRC}/install-prefix \
        -DNNG_ELIDE_DEPRECATED=ON -DNNG_TESTS=OFF -DNNG_TOOLS=OFF \
        -DCMAKE_INSTALL_PREFIX=${SRC}/install-prefix .. \
    && make -j2 \
    && make install

RUN mkdir -p ${RUNTIME_NATIVE} \
    && cp ${SRC}/install-prefix/lib/libnng.so \
        ${SRC}/install-prefix/lib/libmbedcrypto.so \
        ${SRC}/install-prefix/lib/libmbedx509.so \
        ${SRC}/install-prefix/lib/libmbedtls.so \
        ${RUNTIME_NATIVE}

################################################################################
# Build ARM32
FROM multiarch/debian-debootstrap:armhf-bullseye AS arm32v7

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get install -y \
        build-essential \
        cmake \
        perl \
        python3 \
        python3-pip \
    && pip3 install Jinja2

ARG SRC
ARG RUNTIME_NATIVE=${SRC}/runtimes/linux-arm/native

WORKDIR ${SRC}
COPY --from=nng ${SRC}/mbedtls ${SRC}/mbedtls
RUN mkdir -p mbedtls/build && cd mbedtls/build \
    && cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release \
        -DUSE_STATIC_MBEDTLS_LIBRARY=OFF \
        -DUSE_SHARED_MBEDTLS_LIBRARY=ON \
        -DENABLE_TESTING=OFF -DENABLE_PROGRAMS=OFF \
        -DCMAKE_INSTALL_PREFIX=${SRC}/install-prefix .. \
    && mkdir -p install \
    && make -j2 \
    && make install

WORKDIR ${SRC}
COPY --from=nng ${SRC}/nng ${SRC}/nng
RUN mkdir -p nng/build && cd nng/build \
    && cmake -G "Unix Makefiles" -DBUILD_SHARED_LIBS=ON \
        -DCMAKE_BUILD_TYPE=Release \
        -DNNG_ENABLE_TLS=ON \
        -DCMAKE_PREFIX_PATH=${SRC}/install-prefix \
        -DNNG_ELIDE_DEPRECATED=ON -DNNG_TESTS=OFF -DNNG_TOOLS=OFF \
        -DCMAKE_INSTALL_PREFIX=${SRC}/install-prefix .. \
    && make -j2 \
    && make install

RUN mkdir -p ${RUNTIME_NATIVE} \
    && cp ${SRC}/install-prefix/lib/libnng.so \
        ${SRC}/install-prefix/lib/libmbedcrypto.so \
        ${SRC}/install-prefix/lib/libmbedx509.so \
        ${SRC}/install-prefix/lib/libmbedtls.so \
        ${RUNTIME_NATIVE}

################################################################################
# Build ARM64
FROM multiarch/ubuntu-core:arm64-focal AS arm64v8

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive TZ=Etc/UTC apt-get install -y \
        build-essential \
        cmake \
        perl \
        python3 \
        python3-pip \
    && pip3 install Jinja2

ARG SRC
ARG RUNTIME_NATIVE=${SRC}/runtimes/linux-arm64/native

WORKDIR ${SRC}
COPY --from=nng ${SRC}/mbedtls ${SRC}/mbedtls
RUN mkdir -p mbedtls/build && cd mbedtls/build \
    && cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release \
        -DUSE_STATIC_MBEDTLS_LIBRARY=OFF \
        -DUSE_SHARED_MBEDTLS_LIBRARY=ON \
        -DENABLE_TESTING=OFF -DENABLE_PROGRAMS=OFF \
        -DCMAKE_INSTALL_PREFIX=${SRC}/install-prefix .. \
    && mkdir -p install \
    && make -j2 \
    && make install

WORKDIR ${SRC}
COPY --from=nng ${SRC}/nng ${SRC}/nng
RUN mkdir -p nng/build && cd nng/build \
    && cmake -G "Unix Makefiles" -DBUILD_SHARED_LIBS=ON \
        -DCMAKE_BUILD_TYPE=Release \
        -DNNG_ENABLE_TLS=ON \
        -DCMAKE_PREFIX_PATH=${SRC}/install-prefix \
        -DNNG_ELIDE_DEPRECATED=ON -DNNG_TESTS=OFF -DNNG_TOOLS=OFF \
        -DCMAKE_INSTALL_PREFIX=${SRC}/install-prefix .. \
    && make -j2 \
    && make install

RUN mkdir -p ${RUNTIME_NATIVE} \
    && cp ${SRC}/install-prefix/lib/libnng.so \
        ${SRC}/install-prefix/lib/libmbedcrypto.so \
        ${SRC}/install-prefix/lib/libmbedx509.so \
        ${SRC}/install-prefix/lib/libmbedtls.so \
        ${RUNTIME_NATIVE}

################################################################################
# Copy binaries to host
FROM ubuntu:focal AS copy

ARG SRC
WORKDIR ${SRC}

COPY --from=nng ${SRC}/mbedtls/LICENSE ./licenses/mbedtls-LICENSE.txt
COPY --from=nng ${SRC}/nng/LICENSE.txt ./licenses/nng-LICENSE.txt

COPY --from=linux-x64 ${SRC}/runtimes ./runtimes
COPY --from=arm32v7 ${SRC}/runtimes ./runtimes
COPY --from=arm64v8 ${SRC}/runtimes ./runtimes
CMD cp -rf ./runtimes/* /runtimes && cp -rf ./licenses/* /licenses
