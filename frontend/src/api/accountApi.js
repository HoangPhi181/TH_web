/*-----------------------dùng cho folder account--------------------*/
import axiosClient from "./axiosClient";

export const account = () => {
    return axiosClient.get("/auth/account");
}

export const openAccount = (data) => {
    return axiosClient.post("/auth/open-account", data);
}

export const profile = () => {
    return axiosClient.get("/auth/profile");
}

export const updateProfile = (data) => {
    return axiosClient.put("/auth/update-profile", data);
}

export const balance = () => {
    return axiosClient.get("/orders/balance")
}

export const history = () => {
    return axiosClient.get("/orders/history/list");
}