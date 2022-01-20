import { plainText } from "./blocks";

export const COLLECT_LEARNING_MODAL_ID = "collect_learning_modal_id";
export const SELECTING_TECH_TAGS_IN_MODAL = "selecting_tech_tags_in_modal";
export const SELECTING_FELLOW_LEARNERS = "selecting_fellow_learners";
export default function createModal(
  triggerId: string,
  userName: string,
  userId: string
) {
  return {
    trigger_id: triggerId,
    view: {
      callback_id: COLLECT_LEARNING_MODAL_ID,
      type: "modal",
      title: plainText("Hva lærer du deg?"),
      submit: plainText("Del :rocket:"),
      close: plainText("Avbryt :wave:"),
      blocks: [
        {
          type: "section",
          text: plainText(
            `:wave: Hei ${userName}!\n\n Jeg lagrer det du har lært og tar det med i en ukentlig oppsummering jeg publiserer i #fag`
          ),
        },
        {
          type: "input",
          label: plainText("Fortell alle hva du lærer om?"),
          block_id: "shareability",
          element: {
            type: "radio_buttons",
            initial_option: {
              text: plainText("Fortell om det i #fag med en gang."),
              value: "all",
            },
            options: [
              {
                text: plainText("Fortell om det i #fag med en gang."),
                value: "all",
              },
              {
                text: plainText("Bare ta det med når uken oppsummeres."),
                value: "summary",
              },
            ],
            action_id: "shareability_radio_buttons_action",
          },
        },
        {
          type: "section",
          block_id: "tags",
          text: {
            type: "mrkdwn",
            text: "Pick items from the list",
          },
          accessory: {
            action_id: SELECTING_TECH_TAGS_IN_MODAL,
            type: "multi_external_select",
            placeholder: {
              type: "plain_text",
              text: "Velg buzz ord",
            },
            min_query_length: 3,
          },
        },
        {
          type: "input",
          block_id: "learners",
          element: {
            type: "multi_users_select",
            placeholder: plainText("Velg koleger"),
            action_id: SELECTING_FELLOW_LEARNERS,
            initial_users: [userId],
          },
          label: plainText("Hvem var det som lærte?"),
        },
        {
          type: "input",
          label: plainText("Hva handler det du lærer deg om?"),
          optional: true,
          block_id: "description",
          element: {
            type: "plain_text_input",
            focus_on_load: true,
            placeholder: plainText("Kodet litt bakend i Alvtime slack appen"),
            multiline: true,
            action_id: "description_input_action",
          },
        },
        {
          type: "input",
          label: plainText("Hvor finnes dette?"),
          optional: true,
          block_id: "locationOfDetails",
          element: {
            type: "plain_text_input",
            min_length: 0,
            placeholder: plainText("Lim in en Url for eksempel"),
            action_id: "locationOfDetails_input_action",
          },
        },
      ],
    },
  };
}
