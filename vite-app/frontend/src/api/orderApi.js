/*------------dùng cho folder trading---------------*/
import axiosClient from "./axiosClient";

export const balance = () => {
    return axiosClient.get("/orders/balance");
};

export const opening = () => {
    return axiosClient.get("/orders/opening");
};

export const close = (orderId, data) => {
    return axiosClient.post(`/orders/${orderId}/close`, data);
};

export const create = (data) => {
    return axiosClient.post("/orders/create", data);
};