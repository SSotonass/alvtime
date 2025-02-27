import { State } from "./index";
import config from "@/config";
import { ActionContext } from "vuex";
import httpClient from "../services/httpClient";

interface VacationOverview {
  availableVacationDays: number;
  plannedVacationDaysThisYear: number;
  usedVacationDaysThisYear: number;
}

interface AbsenseOverview {
  absenseDaysInAYear: number;
  usedAbsenseDays: number;
}

export interface AbsenseState {
  absenseState: AbsenseStateModel;
}

export interface AbsenseStateModel {
  vacationOverview: VacationOverview;
  absenseOverview: AbsenseOverview;
}

const initState: AbsenseStateModel = {
  vacationOverview: {
    usedVacationDaysThisYear: 0,
    plannedVacationDaysThisYear: 0,
    availableVacationDays: 0,
  },
  absenseOverview: {
    absenseDaysInAYear: 0,
    usedAbsenseDays: 0,
  },
};

const state: AbsenseState = {
  absenseState: initState,
};

const getters = {
  getAbsenseOverview: (state: State) => {
    return [
      {
        name: "Planlagt i år",
        colorValue: "#00B050",
        value: state.absenseState.vacationOverview.plannedVacationDaysThisYear,
        priority: 2,
      },
      {
        name: "Brukt i år",
        colorValue: "#E8B925",
        value: state.absenseState.vacationOverview.usedVacationDaysThisYear,
        priority: 1,
      },
      {
        name: "Tilgjengelig",
        colorValue: "#1D92CE",
        value: state.absenseState.vacationOverview.availableVacationDays,
        priority: 3,
      },
    ];
  },
};

const mutations = {
  SET_ABSENSEDAYSOVERVIEW(state: State, absenseOverview: AbsenseOverview) {
    state.absenseState.absenseOverview = absenseOverview;
  },
  SET_VACATIONOVERVIEW(state: State, vacationOverview: VacationOverview) {
    state.absenseState.vacationOverview = vacationOverview;
  },
};

const actions = {
  FETCH_ABSENSEDATAOVERVIEW: ({ commit }: ActionContext<State, State>) => {
    return httpClient
      .get(`${config.API_HOST}/api/user/AbsenseOverview`)
      .then(response => {
        commit("SET_ABSENSEDAYSOVERVIEW", response.data);
      });
  },
  FETCH_VACATIONOVERVIEW: ({ commit }: ActionContext<State, State>) => {
    return httpClient
      .get(`${config.API_HOST}/api/user/VacationOverview`)
      .then(response => {
        commit("SET_VACATIONOVERVIEW", response.data);
      });
  },
};

export default {
  state,
  getters,
  mutations,
  actions,
};
